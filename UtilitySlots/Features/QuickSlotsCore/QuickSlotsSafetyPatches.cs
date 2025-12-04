using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Patchs de sécurité pour éviter les IndexOutOfRange dans QuickSlots.
    /// On vérifie les index avant d'appeler les méthodes internes, mais
    /// on évite le spam de logs et on laisse autant que possible la logique
    /// vanilla s'exécuter.
    /// </summary>
    [HarmonyPatch(typeof(QuickSlots))]
    public static class QuickSlotsSafetyPatches
    {
        private static readonly FieldInfo BindingField =
            AccessTools.Field(typeof(QuickSlots), "binding");

        private static readonly FieldInfo SlotCountField =
            AccessTools.Field(typeof(QuickSlots), "slotCount");

        private static readonly FieldInfo ActiveSlotField =
            AccessTools.Field(typeof(QuickSlots), "activeSlot");

        private static readonly FieldInfo DesiredSlotField =
            AccessTools.Field(typeof(QuickSlots), "desiredSlot");

        /// <summary>
        /// Renvoie un nombre de slots logique safe basé sur GetSlotCount()
        /// et la longueur du tableau binding.
        /// </summary>
        private static int GetSafeSlotCount(QuickSlots __instance, InventoryItem[] binding)
        {
            int physical = binding?.Length ?? 0;

            int logical;
            try
            {
                logical = __instance.GetSlotCount();
            }
            catch
            {
                logical = physical;
            }

            if (logical < 0)
                logical = 0;
            if (logical > physical)
                logical = physical;

            return logical;
        }

        /// <summary>
        /// Sécurité avant SelectInternal(int):
        /// on s'assure que slotID est dans [0; safeSlotCount[
        /// et dans [0; binding.Length[. Sinon on reset et on SKIPPE
        /// (pour éviter un IndexOutOfRange violent).
        /// </summary>
        [HarmonyPatch("SelectInternal")]
        [HarmonyPrefix]
        private static bool SelectInternal_Prefix(QuickSlots __instance, ref int slotID)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];

            if (binding == null)
            {
                // On ne sait rien faire, on laisse la méthode originale décider.
                return true;
            }

            int length = binding.Length;
            int safeCount = GetSafeSlotCount(__instance, binding);

            // Corriger un slotCount interne incohérent si besoin.
            if (SlotCountField != null)
            {
                object raw = SlotCountField.GetValue(__instance);
                if (raw is int rawCount && (rawCount < 0 || rawCount > length))
                {
                    Log.Warn($"[UtilitySlots][Quickslots][Safety] slotCount field={rawCount} out of [0;{length}], correcting to {safeCount}.");
                    SlotCountField.SetValue(__instance, safeCount);
                }
            }

            if (safeCount <= 0)
            {
                // Aucun slot safe : on force la désélection et on ne laisse pas SelectInternal
                // accéder à un index invalide.
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                Log.Warn("[UtilitySlots][Quickslots][Safety] safeCount <= 0, skipping SelectInternal.");
                return false;
            }

            // Index hors borne logique (GetSlotCount)
            if (slotID < 0 || slotID >= safeCount)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] invalid slot index {slotID} (safeCount={safeCount}, len={length}). Forcing deselect.");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false;
            }

            // Garde-fou physique (au cas où un autre tableau interne serait plus court)
            if (slotID >= length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] slotID={slotID} >= binding.Length={length}. Forcing deselect.");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false;
            }

            // Tout est OK, on laisse la méthode originale travailler.
            return true;
        }

        /// <summary>
        /// Sécurité avant DeselectInternal():
        /// on vérifie que activeSlot est dans [0; safeSlotCount[,
        /// sinon on le remet à -1. On NE BLOQUE PAS la méthode originale.
        /// </summary>
        [HarmonyPatch("DeselectInternal")]
        [HarmonyPrefix]
        private static bool DeselectInternal_Prefix(QuickSlots __instance)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];

            if (binding == null)
            {
                // Pas d'info -> on laisse la méthode vanilla.
                return true;
            }

            int length = binding.Length;
            int safeCount = GetSafeSlotCount(__instance, binding);

            int activeSlot = -1;
            if (ActiveSlotField != null)
            {
                object raw = ActiveSlotField.GetValue(__instance);
                if (raw is int i)
                    activeSlot = i;
            }

            // Si rien n'est sélectionné, on ne touche à rien et on laisse la méthode vanilla.
            if (activeSlot < 0)
                return true;

            // Si l'index est incohérent, on le reset pour éviter un accès hors borne,
            // puis on laisse DeselectInternal faire son travail logique.
            if (activeSlot >= safeCount || activeSlot >= length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] activeSlot={activeSlot} out of range (safeCount={safeCount}, len={length}), resetting to -1.");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
            }

            return true; // Toujours laisser la méthode originale s'exécuter
        }
    }
}

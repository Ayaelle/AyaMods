using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Patchs de sécurité pour éviter les IndexOutOfRange dans QuickSlots.
    /// On vérifie que les index sont cohérents avec la longueur des tableaux internes.
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
        /// S’assure que slotCount ne dépasse pas la taille du tableau,
        /// et que slotID est dans les bornes. Si ce n’est pas le cas,
        /// on force un “reset” des slots sélectionnés.
        /// </summary>
        private static bool EnsureValidIndex(QuickSlots __instance, ref int slotID)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];
            if (binding == null)
                return false;

            int length = binding.Length;
            int slotCount = (int)(SlotCountField?.GetValue(__instance) ?? length);

            // Si slotCount est plus grand que le tableau, on le réduit.
            if (slotCount > length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] slotCount={slotCount} > binding.Length={length}, correction.");
                slotCount = length;
                SlotCountField?.SetValue(__instance, slotCount);
            }

            // Si l’index demandé est hors bornes, on annule et on nettoie l’état.
            if (slotID < 0 || slotID >= length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] invalid slot index {slotID} (len={length}). Forcing deselect.");

                // On remet l’état QuickSlots dans un truc safe.
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);

                // On empêche l’exécution du SelectInternal original.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sécurité avant SelectInternal(slotID).
        /// </summary>
        [HarmonyPatch("SelectInternal")]
        [HarmonyPrefix]
        private static bool SelectInternal_Prefix(QuickSlots __instance, ref int slotID)
        {
            return EnsureValidIndex(__instance, ref slotID);
        }

        /// <summary>
        /// Sécurité avant DeselectInternal() :
        /// on vérifie que activeSlot est dans les bornes,
        /// sinon on le remet à -1 et on skippe.
        /// </summary>
        [HarmonyPatch("DeselectInternal")]
        [HarmonyPrefix]
        private static bool DeselectInternal_Prefix(QuickSlots __instance)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];
            if (binding == null)
                return true; // laisser faire la méthode vanilla

            int length = binding.Length;
            int activeSlot = (int)(ActiveSlotField?.GetValue(__instance) ?? -1);
            int slotCount = (int)(SlotCountField?.GetValue(__instance) ?? length);

            if (slotCount > length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] slotCount={slotCount} > binding.Length={length} in DeselectInternal, correction.");
                slotCount = length;
                SlotCountField?.SetValue(__instance, slotCount);
            }

            if (activeSlot >= length)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] activeSlot={activeSlot} >= binding.Length={length}, resetting.");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false; // rien à désélectionner, on skippe
            }

            return true; // OK, laisser faire la méthode originale
        }
    }
}

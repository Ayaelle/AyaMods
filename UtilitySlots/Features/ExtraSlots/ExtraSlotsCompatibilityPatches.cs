using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patches de compatibilité bas niveau pour les chip slots :
    /// - Étend Equipment.slotMapping pour Chip3..Chip6 -> EquipmentType.Chip
    /// - Étend Equipment.GetSlots pour que les items de type Chip puissent utiliser Chip3..ChipN
    /// - Sécurise Equipment.AddItem pour que le dictionnaire interne ait bien les clés Chip3/Chip4/Chip5/Chip6.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsCompatibilityPatches
    {
        // slotMapping global (static Dictionary<string, EquipmentType>)
        private static readonly FieldInfo SlotMappingField =
            AccessTools.Field(typeof(Equipment), "slotMapping");

        // D’autres mods peuvent vouloir aller jusqu’à 6 ; nous, on clamp runtime à 4 pour l’instant.
        internal static readonly string[] ExtraChipSlots =
        {
            "Chip3",
            "Chip4",
            "Chip5",
            "Chip6"
        };

        /// <summary>
        /// Ajoute Chip1..Chip6 dans Equipment.slotMapping en tant que EquipmentType.Chip.
        /// Appelé après le constructeur statique de Equipment.
        /// </summary>
        internal static void EnsureGlobalChipSlotMapping()
        {
            try
            {
                if (SlotMappingField == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][Compat] Equipment.slotMapping field not found; chip compat patch skipped.");
                    return;
                }

                var dict = SlotMappingField.GetValue(null) as Dictionary<string, EquipmentType>;
                if (dict == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][Compat] Equipment.slotMapping is null; chip compat patch skipped.");
                    return;
                }

                void EnsureChipSlot(string slotId)
                {
                    if (!dict.ContainsKey(slotId))
                    {
                        dict[slotId] = EquipmentType.Chip;
                        Log.Info($"[UtilitySlots][ExtraSlots][Compat] slotMapping['{slotId}'] -> EquipmentType.Chip");
                    }
                }

                // On sécurise Chip1/Chip2 et nos slots extra
                EnsureChipSlot("Chip1");
                EnsureChipSlot("Chip2");
                foreach (var slotId in ExtraChipSlots)
                    EnsureChipSlot(slotId);
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Compat] Exception in EnsureGlobalChipSlotMapping: " + e);
            }
        }

        /// <summary>
        /// Postfix sur Equipment.GetSlots(EquipmentType itemType, List<string> results)
        /// pour ajouter Chip3..ChipN comme slots possibles pour les items de type Chip.
        /// C’est ce que le jeu utilise pour auto-sélectionner un slot quand on clique sur un item.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetSlots))]
        private static class Equipment_GetSlots_Patch
        {
            static void Postfix(EquipmentType itemType, List<string> results)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    if (itemType != EquipmentType.Chip)
                        return;

                    if (results == null)
                        return;

                    int desired = ExtraSlotsRuntime.GetDesiredChipSlots();

                    for (int i = 3; i <= desired && i <= ExtraSlotsRuntime.MaxChipSlots; i++)
                    {
                        string slotId = $"Chip{i}";
                        if (!results.Contains(slotId))
                        {
                            results.Add(slotId);
                            Log.Info($"[UtilitySlots][ExtraSlots][Compat] GetSlots: added '{slotId}' for itemType={itemType}.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlots][Compat] Exception in Equipment.GetSlots postfix: " + e);
                }
            }
        }

        /// <summary>
        /// Prefix sur Equipment.AddItem(string slot, InventoryItem newItem, bool forced = false)
        /// pour s’assurer que le dictionnaire interne this.equipment a bien une entrée pour Chip3/4/5/6.
        /// Sans ça, le jeu loggue des erreurs quand on essaie de placer quelque chose dans un slot inconnu.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.AddItem))]
        private static class Equipment_AddItem_Patch
        {
            static void Prefix(Equipment __instance, string slot)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    if (string.IsNullOrEmpty(slot))
                        return;

                    if (!slot.StartsWith("Chip", StringComparison.Ordinal))
                        return;

                    if (!int.TryParse(slot.Substring(4), out int index))
                        return;

                    if (index <= ExtraSlotsRuntime.VanillaChipSlots)
                        return; // Chip1/2 = vanilla, laisser tranquille

                    // Récupère le dico privé equipment : Dictionary<string, InventoryItem>
                    var eqField = AccessTools.Field(typeof(Equipment), "equipment");
                    var dict = eqField?.GetValue(__instance) as Dictionary<string, InventoryItem>;
                    if (dict == null)
                        return;

                    if (!dict.ContainsKey(slot))
                    {
                        dict[slot] = null;
                        Log.Info($"[UtilitySlots][ExtraSlots][Compat] AddItem Prefix: adding missing key '{slot}' to equipment dict.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlots][Compat] Exception in Equipment.AddItem prefix: " + e);
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlotsCore
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
        private static readonly FieldInfo SlotMappingField =
            AccessTools.Field(typeof(Equipment), "slotMapping");

        internal static readonly string[] ExtraChipSlots =
        {
            "Chip3",
            "Chip4",
            "Chip5",
            "Chip6"
        };

        internal static void EnsureGlobalChipSlotMapping()
        {
            try
            {
                if (SlotMappingField == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsCore][Compat] Equipment.slotMapping field not found; chip compat patch skipped.");
                    return;
                }

                var dict = SlotMappingField.GetValue(null) as Dictionary<string, EquipmentType>;
                if (dict == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsCore][Compat] Equipment.slotMapping is null; chip compat patch skipped.");
                    return;
                }

                void EnsureChipSlot(string slotId)
                {
                    if (!dict.ContainsKey(slotId))
                    {
                        dict[slotId] = EquipmentType.Chip;
                        Log.Info($"[UtilitySlots][ExtraSlotsCore][Compat] slotMapping['{slotId}'] -> EquipmentType.Chip");
                    }
                }

                EnsureChipSlot("Chip1");
                EnsureChipSlot("Chip2");
                foreach (var slotId in ExtraChipSlots)
                    EnsureChipSlot(slotId);
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsCore][Compat] Exception in EnsureGlobalChipSlotMapping: " + e);
            }
        }

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
                            Log.Info($"[UtilitySlots][ExtraSlotsCore][Compat] GetSlots: added '{slotId}' for itemType={itemType}.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsCore][Compat] Exception in Equipment.GetSlots postfix: " + e);
                }
            }
        }

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
                        return; // Chip1/2 = vanilla

                    var eqField = AccessTools.Field(typeof(Equipment), "equipment");
                    var dict = eqField?.GetValue(__instance) as Dictionary<string, InventoryItem>;
                    if (dict == null)
                        return;

                    if (!dict.ContainsKey(slot))
                    {
                        dict[slot] = null;
                        Log.Info($"[UtilitySlots][ExtraSlotsCore][Compat] AddItem Prefix: adding missing key '{slot}' to equipment dict.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsCore][Compat] Exception in Equipment.AddItem prefix: " + e);
                }
            }
        }
    }
}

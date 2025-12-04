using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsCore
{
    /// <summary>
    /// Patches de compatibilité bas niveau pour ExtraSlots :
    /// - Étend Equipment.slotMapping pour Chip3..Chip6 -> EquipmentType.Chip
    /// - Étend Equipment.slotMapping pour SeamothModule5..12 -> EquipmentType.SeamothModule
    ///   et ExosuitModule5..12 -> EquipmentType.ExosuitModule
    ///   et Module7..14 (Cyclops) -> EquipmentType.CyclopsModule
    /// - Étend Equipment.GetSlots pour que les items de type Chip puissent utiliser Chip3..ChipN
    /// - Sécurise Equipment.AddItem pour que le dictionnaire interne ait bien les clés Chip3/Chip4/Chip5/Chip6.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsCompatibilityPatches
    {
        private static readonly FieldInfo SlotMappingField =
            AccessTools.Field(typeof(Equipment), "slotMapping");

        // -----------------------
        // BASES DÉCLARATIVES
        // -----------------------

        internal static readonly string[] ExtraChipSlots =
        {
            "Chip3",
            "Chip4",
            "Chip5",
            "Chip6"
        };

        /// <summary>
        /// Slots de modules "extra" pour le Seamoth (au-delà des 4 vanilla).
        /// </summary>
        internal static readonly string[] ExtraSeamothModuleSlots =
        {
            "SeamothModule5",
            "SeamothModule6",
            "SeamothModule7",
            "SeamothModule8",
            "SeamothModule9",
            "SeamothModule10",
            "SeamothModule11",
            "SeamothModule12"
        };

        /// <summary>
        /// Slots de modules "extra" pour l’Exosuit (au-delà des 4 vanilla).
        /// </summary>
        internal static readonly string[] ExtraExosuitModuleSlots =
        {
            "ExosuitModule5",
            "ExosuitModule6",
            "ExosuitModule7",
            "ExosuitModule8",
            "ExosuitModule9",
            "ExosuitModule10",
            "ExosuitModule11",
            "ExosuitModule12"
        };

        /// <summary>
        /// Slots de modules "extra" pour le Cyclops (au-delà des 6 vanilla).
        /// </summary>
        internal static readonly string[] ExtraCyclopsModuleSlots =
        {
            "Module7",
            "Module8",
            "Module9",
            "Module10",
            "Module11",
            "Module12",
            "Module13",
            "Module14"
        };

        /// <summary>
        /// Initialise / complète le dictionnaire global Equipment.slotMapping pour :
        /// - Chip1..Chip6
        /// - SeamothModule5..12
        /// - ExosuitModule5..12
        /// - Module7..14 (Cyclops)
        /// </summary>
        internal static void EnsureGlobalChipSlotMapping()
        {
            try
            {
                if (SlotMappingField == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsCore][Compat] Equipment.slotMapping field not found; compat patch skipped.");
                    return;
                }

                var dict = SlotMappingField.GetValue(null) as Dictionary<string, EquipmentType>;
                if (dict == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsCore][Compat] Equipment.slotMapping is null; compat patch skipped.");
                    return;
                }

                void EnsureSlot(string slotId, EquipmentType type)
                {
                    if (!dict.ContainsKey(slotId))
                    {
                        dict[slotId] = type;
                        Log.Info($"[UtilitySlots][ExtraSlotsCore][Compat] slotMapping['{slotId}'] -> EquipmentType.{type}");
                    }
                }

                // --- Chips (joueur) ---

                EnsureSlot("Chip1", EquipmentType.Chip);
                EnsureSlot("Chip2", EquipmentType.Chip);
                foreach (var slotId in ExtraChipSlots)
                    EnsureSlot(slotId, EquipmentType.Chip);

                // --- Véhicules : Seamoth modules ---

                // On se limite aux slots déclarés dans ExtraSeamothModuleSlots, qui
                // correspondent à Vanilla=4 -> Max=12.
                foreach (var slotId in ExtraSeamothModuleSlots)
                    EnsureSlot(slotId, EquipmentType.SeamothModule);

                // --- Véhicules : Exosuit modules ---

                foreach (var slotId in ExtraExosuitModuleSlots)
                    EnsureSlot(slotId, EquipmentType.ExosuitModule);

                // --- Véhicules : Cyclops modules ---

                foreach (var slotId in ExtraCyclopsModuleSlots)
                    EnsureSlot(slotId, EquipmentType.CyclopsModule);
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

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
    /// - Étend Equipment.slotMapping pour SeamothModule5..N -> EquipmentType.SeamothModule
    ///   et ExosuitModule5..N -> EquipmentType.ExosuitModule
    /// - Étend Equipment.GetSlots pour que les items de type Chip puissent utiliser Chip3..ChipN
    /// - Sécurise Equipment.AddItem pour que le dictionnaire interne ait bien les clés Chip3/Chip4/Chip5/Chip6.
    /// 
    /// Remarque :
    /// Pour les véhicules, le mapping est global (static) et couvre directement toute la plage
    /// jusqu’aux MaxSeamothModuleSlots / MaxExosuitModuleSlots, ce qui évite les erreurs
    /// "Slot type is not defined in Equipment.slotMapping dictionary." lors des AddSlot().
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

        /// <summary>
        /// Initialise / complète le dictionnaire global Equipment.slotMapping pour :
        /// - Chip1..Chip6
        /// - SeamothModule* supplémentaires
        /// - ExosuitModule* supplémentaires
        /// 
        /// Appelé une fois au démarrage de la feature ExtraSlots.
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

                // Dans Subnautica, slotMapping est un champ static sur Equipment.
                // On utilise GetValue(null) (comme pour les chips auparavant).
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

                // On map directement tous les slots possibles jusqu’au max défini dans ExtraSlotsVehiclesRuntime.
                // Cela évite des erreurs AddSlot() pour les slots au-delà de la config actuelle (et ne gêne pas le vanilla).
                int seamothMax = ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots;
                for (int i = ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots + 1; i <= seamothMax; i++)
                {
                    string slotId = $"SeamothModule{i}";
                    EnsureSlot(slotId, EquipmentType.SeamothModule);
                }

                // --- Véhicules : Exosuit modules ---

                int exoMax = ExtraSlotsVehiclesRuntime.MaxExosuitModuleSlots;
                for (int i = ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1; i <= exoMax; i++)
                {
                    string slotId = $"ExosuitModule{i}";
                    EnsureSlot(slotId, EquipmentType.ExosuitModule);
                }
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

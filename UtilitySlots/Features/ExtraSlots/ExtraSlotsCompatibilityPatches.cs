using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patches / helpers de compat pour ExtraSlots.
    /// - Étend Equipment.slotMapping pour déclarer Chip3..Chip6 comme slots de type Chip.
    /// - S’assure que le dictionnaire interne "equipment" contient bien Chip3..Chip6
    ///   au moment où AddItem est appelé, pour éviter que AddItem retourne false.
    ///
    /// IMPORTANT : on ne patch PLUS le constructeur statique de Equipment.
    /// On appelle explicitement EnsureGlobalChipSlotMapping() au démarrage de la feature.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsCompatibilityPatches
    {
        // Champ privé static readonly Dictionary<string, EquipmentType> slotMapping
        private static readonly FieldInfo SlotMappingField =
            AccessTools.Field(typeof(Equipment), "slotMapping");

        // Champ privé Dictionary<string, InventoryItem> equipment (par instance)
        private static readonly FieldInfo EquipmentDictField =
            AccessTools.Field(typeof(Equipment), "equipment");

        private static readonly string[] ExtraChipSlots =
        {
            "Chip3",
            "Chip4",
            "Chip5",
            "Chip6"
        };

        /// <summary>
        /// Approche "mod tier" :
        /// on étend le mapping global slotId -> EquipmentType UNE SEULE FOIS,
        /// en étant appelé depuis ExtraSlotsFeature.Enable().
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

                // On force Chip1/2/3/4/5/6 à être de type Chip (sécurise aussi d’éventuels autres mods)
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

        // --------------------------------------------------------------------
        // 2) Garantir que le dictionnaire "equipment" contient bien Chip3..Chip6
        //    juste avant l’exécution de AddItem.
        //
        //    AddItem vanilla :
        //      if (!this.equipment.TryGetValue(slot, out inventoryItem)) return false;
        //
        //    Donc si la clé "Chip3" n’existe pas dans le dictionnaire d’instance,
        //    AddItem retourne false DIRECT, même si slotMapping est OK.
        // --------------------------------------------------------------------
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.AddItem))]
        private static class Equipment_AddItem_Patch
        {
            // Signature: bool AddItem(string slot, InventoryItem newItem, bool forced = false)
            static void Prefix(Equipment __instance, string slot, InventoryItem newItem, bool forced)
            {
                try
                {
                    if (newItem == null)
                        return;

                    // On ne s’intéresse qu’à nos extra slots de puce
                    bool isExtraChip =
                        slot == "Chip3" ||
                        slot == "Chip4" ||
                        slot == "Chip5" ||
                        slot == "Chip6";

                    if (!isExtraChip)
                        return;

                    if (EquipmentDictField == null)
                    {
                        Log.Warn("[UtilitySlots][ExtraSlots][Compat] Equipment.equipment field not found; cannot auto-create chip slot.");
                        return;
                    }

                    var dict = EquipmentDictField.GetValue(__instance) as Dictionary<string, InventoryItem>;
                    if (dict == null)
                    {
                        Log.Warn("[UtilitySlots][ExtraSlots][Compat] Equipment.equipment is null; cannot auto-create chip slot.");
                        return;
                    }

                    // Si le dictionnaire n’a pas de clé "Chip3"/"Chip4"/..., AddItem vanilla renverra false.
                    if (!dict.ContainsKey(slot))
                    {
                        Log.Info($"[UtilitySlots][ExtraSlots][Compat] AddItem Prefix: adding missing key '{slot}' to equipment dict.");
                        dict[slot] = null; // slot vide, comme le ferait AddSlot
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlots][Compat] Exception in Equipment.AddItem prefix: " + e);
                }
            }
        }

        // NB : on NE patch plus uGUI_Equipment.CanSwitchOrSwap, ni Equipment.AllowedToAdd.
        // Avec slotMapping étendu + dict d’instance complet, la compatibilité suit le chemin vanilla.
    }
}

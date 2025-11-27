using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine; // juste pour Debug.Log si besoin

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patches de compatibilité "bas niveau" pour ExtraSlots.
    /// - Étend Equipment.slotMapping pour déclarer Chip3..Chip6 comme slots de type Chip.
    /// - Ajuste la compatibilité UI (CanSwitchOrSwap) pour traiter Chip3..Chip6 comme Chip1.
    /// - Étend Equipment.IsCompatible pour logger et éventuellement élargir la compat.
    /// - Log agressif sur GetSlotType pour Chip1..Chip4.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsCompatibilityPatches
    {
        // Champ privé static readonly Dictionary<string, EquipmentType> slotMapping
        private static readonly FieldInfo SlotMappingField =
            AccessTools.Field(typeof(Equipment), "slotMapping");

        /// <summary>
        /// Postfix sur le constructeur statique de Equipment.
        /// C’est ici que Subnautica initialise slotMapping.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), MethodType.StaticConstructor)]
        [HarmonyPostfix]
        private static void Equipment_StaticCtor_Postfix()
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
                    else
                    {
                        Log.Info($"[UtilitySlots][ExtraSlots][Compat] slotMapping already has '{slotId}' = {dict[slotId]}");
                    }
                }

                // On s’assure que toutes nos puces sont déclarées comme type Chip
                EnsureChipSlot("Chip1");
                EnsureChipSlot("Chip2");
                EnsureChipSlot("Chip3");
                EnsureChipSlot("Chip4");
                EnsureChipSlot("Chip5");
                EnsureChipSlot("Chip6");

                // Dump rapide pour debug
                Log.Info("[UtilitySlots][ExtraSlots][Compat] Dump slotMapping for Chip1..6 : " +
                         $"Chip1={dict["Chip1"]}, Chip2={dict["Chip2"]}, " +
                         $"Chip3={dict.GetValueOrDefault("Chip3", EquipmentType.None)}, " +
                         $"Chip4={dict.GetValueOrDefault("Chip4", EquipmentType.None)}, " +
                         $"Chip5={dict.GetValueOrDefault("Chip5", EquipmentType.None)}, " +
                         $"Chip6={dict.GetValueOrDefault("Chip6", EquipmentType.None)}");
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Compat] Exception in Equipment static ctor postfix: " + e);
            }
        }

        /// <summary>
        /// Log agressif sur Equipment.GetSlotType pour Chip1..Chip4.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetSlotType))]
        private static class GetSlotType_LogPatch
        {
            static void Postfix(string slot, ref EquipmentType __result)
            {
                if (slot == "Chip1" || slot == "Chip2" ||
                    slot == "Chip3" || slot == "Chip4" ||
                    slot == "Chip5" || slot == "Chip6")
                {
                    Log.Info($"[UtilitySlots][ExtraSlots][Compat] GetSlotType('{slot}') -> {__result}");
                }
            }
        }

        /// <summary>
        /// Patch logique : Equipment.IsCompatible
        ///
        /// On log systématiquement ce qui est testé. Pour les slots de type Chip,
        /// on ajoute éventuellement une compat étendue.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.IsCompatible))]
        private static class ChipExtendedIsCompatiblePatch
        {
            static bool Prefix(EquipmentType itemType, EquipmentType slotType, ref bool __result)
            {
                // Log agressif
                Log.Info($"[UtilitySlots][ExtraSlots][Compat] IsCompatible? itemType={itemType}, slotType={slotType}");

                // On ne touche qu’aux slots de type Chip, sinon vanilla
                if (slotType != EquipmentType.Chip)
                    return true; // laisser vanilla faire

                // Items "Chip" dans slot "Chip" : OK (comportement vanilla)
                if (itemType == EquipmentType.Chip)
                {
                    Log.Info("[UtilitySlots][ExtraSlots][Compat] IsCompatible: Chip -> Chip => TRUE (forced)");
                    __result = true;
                    return false;
                }

                // Option : autoriser aussi Head dans Chip (pour la boussole si elle est Head)
                if (itemType == EquipmentType.Head)
                {
                    Log.Info("[UtilitySlots][ExtraSlots][Compat] IsCompatible: Head -> Chip => TRUE (extended)");
                    __result = true;
                    return false;
                }

                // Pour tout le reste, laisser vanilla décider
                Log.Info("[UtilitySlots][ExtraSlots][Compat] IsCompatible: non-special combination, falling back to vanilla.");
                return true;
            }
        }

        /// <summary>
        /// Patch UI : uGUI_Equipment.CanSwitchOrSwap
        /// Si le slotB est Chip3..Chip6, on traite sa compatibilité comme Chip1,
        /// et on log le tout.
        /// </summary>
        [HarmonyPatch(typeof(uGUI_Equipment), "CanSwitchOrSwap")]
        private static class ChipAliasForUICompatibilityPatch
        {
            static bool Prefix(
                uGUI_Equipment __instance,
                ref ItemAction __result,
                string slotB)
            {
                // On ne s’intéresse qu’à nos slots extra Chip
                if (slotB != "Chip3" && slotB != "Chip4" &&
                    slotB != "Chip5" && slotB != "Chip6")
                {
                    return true; // vanilla
                }

                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] CanSwitchOrSwap for extra chip slot '{slotB}'.");

                if (!ItemDragManager.isDragging)
                {
                    Log.Info("[UtilitySlots][ExtraSlots][PlayerUI]  -> No drag in progress, letting vanilla handle.");
                    return true;
                }

                var draggedItem = ItemDragManager.draggedItem;
                if (draggedItem == null || draggedItem.item == null)
                {
                    Log.Info("[UtilitySlots][ExtraSlots][PlayerUI]  -> No dragged item, letting vanilla handle.");
                    return true;
                }

                // Accès au Equipment associé au uGUI_Equipment
                var eqField = AccessTools.Field(typeof(uGUI_Equipment), "equipment");
                var equipment = eqField?.GetValue(__instance) as Equipment;
                if (equipment == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI]  -> Equipment is null, letting vanilla handle.");
                    return true;
                }

                var item = draggedItem.item;
                TechType techType = item.GetTechType();
                EquipmentType itemType = TechData.GetEquipmentType(techType);
                EquipmentType chip1SlotType = Equipment.GetSlotType("Chip1");
                EquipmentType slotType = Equipment.GetSlotType(slotB);

                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  Dragged item techType={techType}, itemType={itemType}, " +
                         $"Chip1SlotType={chip1SlotType}, SlotType[{slotB}]={slotType}");

                // Compatibilité "logique" : on demande si itemType est compatible avec Chip1
                bool compatibleWithChip =
                    Equipment.IsCompatible(itemType, chip1SlotType);

                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  Equipment.IsCompatible(itemType, Chip1SlotType) => {compatibleWithChip}");

                if (!compatibleWithChip)
                {
                    __result = ItemAction.None;
                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  -> Not compatible with Chip1 logic, returning None.");
                    return false;
                }

                // Mirror du comportement vanilla pour Switch/Swap
                InventoryItem itemInSlot = equipment.GetItemInSlot(slotB);
                if (itemInSlot == null)
                {
                    __result = ItemAction.Switch;
                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  -> Slot '{slotB}' empty, returning SWITCH.");
                }
                else if (Inventory.CanSwap(draggedItem, itemInSlot))
                {
                    __result = ItemAction.Swap;
                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  -> Slot '{slotB}' occupied, CAN SWAP => SWAP.");
                }
                else
                {
                    __result = ItemAction.None;
                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI]  -> Slot '{slotB}' occupied, CANNOT SWAP => NONE.");
                }

                return false; // on a tout géré nous-même
            }
        }
    }

    // Petit helper pour éviter les KeyNotFound dans le log de slotMapping.
    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(
            this Dictionary<TKey, TValue> dict,
            TKey key,
            TValue defaultValue = default)
        {
            if (dict == null)
                return defaultValue;
            TValue value;
            return dict.TryGetValue(key, out value) ? value : defaultValue;
        }
    }
}

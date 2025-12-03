using System;
using AyaCoreMod.Core;
using HarmonyLib;
using UtilitySlots.Features.ExtraSlotsCore;

namespace UtilitySlots.Features.ExtraSlotsPlayer
{
    /// <summary>
    /// Patches côté joueur (Inventory.main)
    /// - À l'Awake de Inventory, on étend les slots de puces.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsPlayerPatches
    {
        [HarmonyPatch(typeof(Inventory), "Awake")]
        [HarmonyPostfix]
        private static void Inventory_Awake_Postfix(Inventory __instance)
        {
            try
            {
                if (!ExtraSlotsRuntime.IsEnabled())
                    return;

                // On ne cible que l'inventaire principal du joueur
                if (__instance != Inventory.main)
                    return;

                if (__instance.equipment == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsCore][Player] Inventory.main.equipment is null in Awake.");
                    return;
                }

                Log.Info("[UtilitySlots][ExtraSlotsCore][Player] Expanding player chip slots…");
                ExtraSlotsPlayerRuntime.ExpandChipSlots(__instance.equipment);
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsCore][Player] Exception in Inventory.Awake postfix: " + e);
            }
        }
    }
}
using System;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlots
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
                    Log.Warn("[UtilitySlots][ExtraSlots][Player] Inventory.main.equipment is null in Awake.");
                    return;
                }

                Log.Info("[UtilitySlots][ExtraSlots][Player] Expanding player chip slots…");
                ExtraSlotsPlayerRuntime.ExpandChipSlots(__instance.equipment);
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Player] Exception in Inventory.Awake postfix: " + e);
            }
        }
    }
}

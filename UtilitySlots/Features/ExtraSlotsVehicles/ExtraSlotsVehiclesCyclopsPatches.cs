using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    /// <summary>
    /// Patches ExtraSlotsVehicles côté Cyclops :
    /// - Étend les slots de la UpgradeConsole (modules Cyclops) via UnlockDefaultModuleSlots.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesCyclopsPatches
    {
        private static bool _loggedCyclopsOnce = false;

        [HarmonyPatch(typeof(UpgradeConsole), "UnlockDefaultModuleSlots")]
        [HarmonyPostfix]
        private static void UpgradeConsole_UnlockDefaultModuleSlots_Postfix(UpgradeConsole __instance)
        {
            try
            {
                if (!ExtraSlotsRuntime.IsEnabled())
                    return;

                var modules = __instance.modules;
                if (modules == null)
                    return;

                int desired = ExtraSlotsVehiclesRuntime.GetDesiredCyclopsModuleSlots();
                if (desired <= ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots)
                    return;

                var slots = new List<string>();

                // Slots vanilla :
                slots.Add("Module1");
                slots.Add("Module2");
                slots.Add("Module3");
                slots.Add("Module4");
                slots.Add("Module5");
                slots.Add("Module6");

                // Slots supplémentaires :
                for (int i = ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots + 1; i <= desired; i++)
                {
                    slots.Add($"Module{i}");
                }

                modules.AddSlots(slots.ToArray());

                if (!_loggedCyclopsOnce)
                {
                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][Cyclops] slots étendus à {desired} modules.");
                    _loggedCyclopsOnce = true;
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][Cyclops] Exception in UnlockDefaultModuleSlots postfix: " + e);
            }
        }
    }
}

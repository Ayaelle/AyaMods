using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Patches ExtraSlotsVehiclesUI pour le Cyclops :
    /// - Étend UpgradeConsole.UnlockedDefaultModuleSlots pour ajouter Module7..N.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesCyclopsPatches
    {
        private static bool _logged;

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

                var extraSlots = new List<string>();

                for (int i = ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots + 1;
                     i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxCyclopsModuleSlots;
                     i++)
                {
                    extraSlots.Add($"Module{i}");
                }

                if (extraSlots.Count > 0)
                {
                    modules.AddSlots(extraSlots.ToArray());

                    if (!_logged)
                    {
                        Log.Info(
                            $"[UtilitySlots][ExtraSlotsVehiclesUI][Cyclops] slots étendus jusqu'à Module{desired} (vanilla={ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots}, max={ExtraSlotsVehiclesRuntime.MaxCyclopsModuleSlots})."
                        );
                        _logged = true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehiclesUI][Cyclops] Exception in UnlockDefaultModuleSlots postfix: " + e);
            }
        }
    }
}

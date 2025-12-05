using AyaCoreMod.Core;
using HarmonyLib;
using System;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    /// <summary>
    /// Patches ExtraSlotsVehicles côté Exosuit :
    /// - Étend Exosuit.slotIDs pour ajouter ExosuitModule5..N (en conservant les bras).
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesExosuitPatches
    {
        private static bool _loggedExosuitOnce = false;

        /// <summary>
        /// Exosuit.slotIDs retourne Exosuit._slotIDs (2 bras + 4 modules).
        /// On remplace la liste par une version étendue pour ExosuitModule5..N.
        /// </summary>
        [HarmonyPatch(typeof(Exosuit), "get_slotIDs")]
        private static class Exosuit_SlotIDs_Patch
        {
            static void Postfix(ref string[] __result)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    int desiredModules = ExtraSlotsVehiclesRuntime.GetDesiredExosuitModuleSlots();
                    if (desiredModules <= ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots)
                        return; // on laisse le vanilla

                    __result = ExtraSlotsVehiclesRuntime.BuildExosuitSlotIDs(desiredModules);

                    if (!_loggedExosuitOnce)
                    {
                        Log.Info($"[UtilitySlots][ExtraSlotsVehicles][Exosuit] slotIDs étendu à {__result.Length} entrées (2 bras + {desiredModules} modules).");
                        _loggedExosuitOnce = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehicles][Exosuit] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }
    }
}

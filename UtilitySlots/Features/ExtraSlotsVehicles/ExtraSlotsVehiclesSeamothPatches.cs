using AyaCoreMod.Core;
using HarmonyLib;
using System;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    /// <summary>
    /// Patches ExtraSlotsVehicles côté Seamoth :
    /// - Étend SeaMoth.slotIDs pour ajouter SeamothModule5..N.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesSeamothPatches
    {
        private static bool _loggedSeamothOnce = false;

        /// <summary>
        /// SeaMoth.slotIDs est surchargé pour retourner SeaMoth._slotIDs (4 modules vanilla).
        /// On post-fixe le getter pour renvoyer un tableau étendu si besoin.
        /// </summary>
        [HarmonyPatch(typeof(SeaMoth), "get_slotIDs")]
        private static class SeaMoth_SlotIDs_Patch
        {
            static void Postfix(ref string[] __result)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    int desired = ExtraSlotsVehiclesRuntime.GetDesiredSeamothModuleSlots();
                    if (desired <= ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots)
                        return; // on laisse le vanilla

                    __result = ExtraSlotsVehiclesRuntime.BuildSeamothSlotIDs(desired);

                    if (!_loggedSeamothOnce)
                    {
                        AyaCoreMod.Core.Log.Info($"[UtilitySlots][ExtraSlotsVehicles][Seamoth] slotIDs étendu à {__result.Length} modules.");
                        _loggedSeamothOnce = true;
                    }
                }
                catch (Exception e)
                {
                    AyaCoreMod.Core.Log.Error("[UtilitySlots][ExtraSlotsVehicles][Seamoth] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }
    }
}

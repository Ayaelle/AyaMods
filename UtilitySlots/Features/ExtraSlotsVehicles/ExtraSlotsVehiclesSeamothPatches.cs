using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Patches ExtraSlotsVehiclesUI pour le Seamoth :
    /// - Étend SeaMoth.slotIDs pour ajouter SeamothModule5..N.
    /// - Étend Equipment.GetSlots pour accepter les nouveaux slots comme cibles valides.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesSeamothPatches
    {
        private static bool _loggedSlotIDsOnce = false;
        private static bool _loggedGetSlotsOnce = false;

        // -----------------------------
        // 1) SeaMoth.slotIDs
        // -----------------------------

        /// <summary>
        /// SeaMoth.slotIDs est surchargé pour renvoyer un tableau étendu si besoin.
        /// Vanilla : 4 modules. Mod : jusqu’à ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots.
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

                    if (!_loggedSlotIDsOnce)
                    {
                        Log.Info($"[UtilitySlots][ExtraSlotsVehiclesUI][Seamoth] slotIDs étendu à {__result.Length} modules.");
                        _loggedSlotIDsOnce = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehiclesUI][Seamoth] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }

        // -----------------------------
        // 2) Equipment.GetSlots : Seamoth
        // -----------------------------

        /// <summary>
        /// Postfix sur Equipment.GetSlots pour ajouter SeamothModule5..N
        /// dans la liste des slots candidats quand le jeu propose des slots pour un module.
        /// On se base sur la présence de "SeamothModule1" dans results.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetSlots))]
        private static class Equipment_GetSlots_Seamoth_Patch
        {
            static void Postfix(EquipmentType itemType, List<string> results)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    if (results == null || results.Count == 0)
                        return;

                    // Seamoth : si la liste contient SeamothModule1, on ajoute les extras.
                    if (results.Contains("SeamothModule1"))
                    {
                        int desired = ExtraSlotsVehiclesRuntime.GetDesiredSeamothModuleSlots();

                        for (int i = ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots + 1;
                             i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots;
                             i++)
                        {
                            string slotId = "SeamothModule" + i;
                            if (!results.Contains(slotId))
                            {
                                results.Add(slotId);

                                if (!_loggedGetSlotsOnce)
                                {
                                    Log.Info(
                                        $"[UtilitySlots][ExtraSlotsVehiclesUI][Seamoth] GetSlots: ajout de slots modules jusqu'à {desired} (max={ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots})."
                                    );
                                    _loggedGetSlotsOnce = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehiclesUI][Seamoth] Exception dans Equipment.GetSlots postfix : " + e);
                }
            }
        }
    }
}

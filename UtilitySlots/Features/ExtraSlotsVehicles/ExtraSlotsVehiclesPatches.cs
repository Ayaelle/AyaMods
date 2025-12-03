using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    /// <summary>
    /// Patches ExtraSlotsVehicles côté véhicules :
    /// - Étend SeaMoth.slotIDs pour ajouter SeamothModule5/6.
    /// - Étend Exosuit.slotIDs pour ajouter ExosuitModule5/6 (en conservant les bras).
    /// - Étend Equipment.GetSlots pour accepter les nouveaux slots comme cibles valides.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesPatches
    {
        // -----------------------------
        // 1) SeaMoth.slotIDs
        // -----------------------------

        /// <summary>
        /// SeaMoth.slotIDs est surchargé pour retourner SeaMoth._slotIDs (4 modules vanilla). :contentReference[oaicite:3]{index=3}
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

                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][Seamoth] slotIDs étendu à {__result.Length} modules.");
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehicles][Seamoth] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }

        // -----------------------------
        // 2) Exosuit.slotIDs
        // -----------------------------

        /// <summary>
        /// Exosuit.slotIDs retourne Exosuit._slotIDs (2 bras + 4 modules). :contentReference[oaicite:4]{index=4}
        /// On remplace la liste par une version étendue pour ExosuitModule5/6.
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

                    Log.Info(
                        $"[UtilitySlots][ExtraSlotsVehicles][Exosuit] slotIDs étendu à {__result.Length} entrées (2 bras + {desiredModules} modules)."
                    );
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehicles][Exosuit] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }

        // -----------------------------
        // 3) Equipment.GetSlots : permettre l’auto-assign sur les nouveaux slots
        // -----------------------------

        /// <summary>
        /// Postfix sur Equipment.GetSlots pour ajouter SeamothModule5/6 ou ExosuitModule5/6
        /// dans la liste des slots candidats quand le jeu propose des slots pour un module.
        /// On ne dépend pas de itemType pour éviter les suppositions sur EquipmentType :
        /// on se base sur la présence de "SeamothModule1" / "ExosuitModule1" dans results.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetSlots))]
        private static class Equipment_GetSlots_VehicleModules_Patch
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
                                Log.Info(
                                    $"[UtilitySlots][ExtraSlotsVehicles][Seamoth] GetSlots: ajout de '{slotId}' pour itemType={itemType}."
                                );
                            }
                        }
                    }

                    // Exosuit : si la liste contient ExosuitModule1, on ajoute les extras.
                    if (results.Contains("ExosuitModule1"))
                    {
                        int desired = ExtraSlotsVehiclesRuntime.GetDesiredExosuitModuleSlots();

                        for (int i = ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1;
                             i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxExosuitModuleSlots;
                             i++)
                        {
                            string slotId = "ExosuitModule" + i;
                            if (!results.Contains(slotId))
                            {
                                results.Add(slotId);
                                Log.Info(
                                    $"[UtilitySlots][ExtraSlotsVehicles][Exosuit] GetSlots: ajout de '{slotId}' pour itemType={itemType}."
                                );
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehicles][Vehicles] Exception dans Equipment.GetSlots postfix : " + e);
                }
            }
        }
    }
}

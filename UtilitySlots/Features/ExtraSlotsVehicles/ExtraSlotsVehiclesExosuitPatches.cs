using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Patches ExtraSlotsVehiclesUI pour l’Exosuit :
    /// - Étend Exosuit.slotIDs pour ajouter ExosuitModule5..N (en conservant les bras).
    /// - Étend Equipment.GetSlots pour accepter les nouveaux slots comme cibles valides.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsVehiclesExosuitPatches
    {
        private static bool _loggedSlotIDsOnce = false;
        private static bool _loggedGetSlotsOnce = false;

        // -----------------------------
        // 1) Exosuit.slotIDs
        // -----------------------------

        /// <summary>
        /// Exosuit.slotIDs retourne un tableau étendu pour les modules.
        /// Vanilla : 2 bras + 4 modules.
        /// Mod : 2 bras + jusqu’à MaxExosuitModuleSlots modules.
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

                    if (!_loggedSlotIDsOnce)
                    {
                        Log.Info(
                            $"[UtilitySlots][ExtraSlotsVehiclesUI][Exosuit] slotIDs étendu à {__result.Length} entrées (2 bras + {desiredModules} modules)."
                        );
                        _loggedSlotIDsOnce = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehiclesUI][Exosuit] Exception dans get_slotIDs postfix : " + e);
                }
            }
        }

        // -----------------------------
        // 2) Equipment.GetSlots : Exosuit
        // -----------------------------

        /// <summary>
        /// Postfix sur Equipment.GetSlots pour ajouter ExosuitModule5..N
        /// dans la liste des slots candidats quand le jeu propose des slots pour un module.
        /// On se base sur la présence de "ExosuitModule1" dans results.
        /// </summary>
        [HarmonyPatch(typeof(Equipment), nameof(Equipment.GetSlots))]
        private static class Equipment_GetSlots_Exosuit_Patch
        {
            static void Postfix(EquipmentType itemType, List<string> results)
            {
                try
                {
                    if (!ExtraSlotsRuntime.IsEnabled())
                        return;

                    if (results == null || results.Count == 0)
                        return;

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

                                if (!_loggedGetSlotsOnce)
                                {
                                    Log.Info(
                                        $"[UtilitySlots][ExtraSlotsVehiclesUI][Exosuit] GetSlots: ajout de slots modules jusqu'à {desired} (max={ExtraSlotsVehiclesRuntime.MaxExosuitModuleSlots})."
                                    );
                                    _loggedGetSlotsOnce = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsVehiclesUI][Exosuit] Exception dans Equipment.GetSlots postfix : " + e);
                }
            }
        }
    }
}

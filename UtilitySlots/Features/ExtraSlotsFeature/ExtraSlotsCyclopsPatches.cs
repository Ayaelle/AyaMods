using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Points d'ancrage pour l'extension des slots module du Cyclops.
    /// </summary>
    [HarmonyPatch]
    public static class ExtraSlotsCyclopsPatches
    {
        /// <summary>
        /// SubRoot est la classe de base pour les sous-marins (Cyclops).
        /// On patch SubRoot.Start pour toucher le Cyclops.
        /// </summary>
        [HarmonyPatch(typeof(SubRoot), "Start")]
        [HarmonyPostfix]
        public static void SubRoot_Start_Postfix(SubRoot __instance)
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            // On ne veut que le Cyclops, pas les Seamoth/Exosuit qui hériteraient éventuellement
            // ou d'autres subroots.
            if (!__instance.isCyclops)
                return;

            int desired = ExtraSlotsRuntime.GetCyclopsModuleSlots();

            try
            {
                Equipment eq = null;

                // À vérifier dans ILSpy : où se trouve l'Equipment pour les modules Cyclops ?
                // Souvent sur un objet "CyclopsUpgradeConsole" ou similaire.
                eq = __instance.GetComponent<Equipment>();
                if (eq == null)
                    eq = __instance.GetComponentInChildren<Equipment>();

                if (eq == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][Cyclops] No Equipment found on Cyclops SubRoot; cannot adjust module slots.");
                    return;
                }

                // 🔹 TODO PATCH RÉEL :
                //
                // Inspecte comment les slots EquipmentType.CyclopsModule
                // sont créés (ex: "CyclopsModule1" .. "CyclopsModule6").
                //
                // Puis étends jusqu'à "desired" :
                //
                // int vanillaMax = 6;
                // for (int i = vanillaMax + 1; i <= desired; i++)
                // {
                //     string slotId = "CyclopsModule" + i;
                //     eq.AddSlot(slotId, EquipmentType.CyclopsModule);
                //     Log.Info($"[UtilitySlots][ExtraSlots][Cyclops] Added module slot {slotId}.");
                // }

                Log.Info($"[UtilitySlots][ExtraSlots][Cyclops] SubRoot.Start detected (Cyclops). Desired module slots = {desired} (vanilla ≈ 6).");
            }
            catch (System.Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Cyclops] Error while adjusting Cyclops module slots: " + e);
            }
        }
    }
}

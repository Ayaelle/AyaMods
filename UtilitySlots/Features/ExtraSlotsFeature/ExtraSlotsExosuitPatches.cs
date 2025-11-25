using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Points d'ancrage pour l'extension des slots module de l'Exosuit (Prawn).
    /// </summary>
    [HarmonyPatch]
    public static class ExtraSlotsExosuitPatches
    {
        [HarmonyPatch(typeof(Exosuit), "Start")]
        [HarmonyPostfix]
        public static void Exosuit_Start_Postfix(Exosuit __instance)
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            int desired = ExtraSlotsRuntime.GetExosuitModuleSlots();

            try
            {
                Equipment eq = null;

                // Idem Seamoth : à confirmer avec ILSpy où se trouve l'Equipment.
                eq = __instance.GetComponent<Equipment>();
                if (eq == null)
                    eq = __instance.GetComponentInChildren<Equipment>();

                if (eq == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][Exosuit] No Equipment found on Exosuit; cannot adjust module slots.");
                    return;
                }

                // 🔹 TODO PATCH RÉEL :
                //
                // Inspecte dans ILSpy comment les slots EquipmentType.ExosuitModule
                // sont créés, typiquement "ExosuitModule1" .. "ExosuitModule4".
                //
                // Puis étends jusqu'à "desired" :
                //
                // int vanillaMax = 4;
                // for (int i = vanillaMax + 1; i <= desired; i++)
                // {
                //     string slotId = "ExosuitModule" + i;
                //     eq.AddSlot(slotId, EquipmentType.ExosuitModule);
                //     Log.Info($"[UtilitySlots][ExtraSlots][Exosuit] Added module slot {slotId}.");
                // }

                Log.Info($"[UtilitySlots][ExtraSlots][Exosuit] Exosuit.Start detected. Desired module slots = {desired} (vanilla ≈ 4).");
            }
            catch (System.Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Exosuit] Error while adjusting Exosuit module slots: " + e);
            }
        }
    }
}

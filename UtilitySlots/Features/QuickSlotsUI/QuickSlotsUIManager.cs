using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Gère les demandes de redessin de la barre QuickSlots.
    /// </summary>
    public static class QuickSlotsUIManager
    {
        private static bool _redrawRequested;

        public static void RequestRedraw()
        {
            _redrawRequested = true;
        }

        private static void ForceReinit(uGUI_QuickSlots instance)
        {
            if (instance == null)
                return;

            var type = typeof(uGUI_QuickSlots);
            var targetField = AccessTools.Field(type, "target");
            var uninitMethod = AccessTools.Method(type, "Uninit");

            if (uninitMethod == null || targetField == null)
            {
                Log.Error("[UtilitySlots][Quickslots][UI] Impossible d'accéder à target/Uninit.");
                return;
            }

            // Cela va :
            // - désabonner les events,
            // - détruire les icônes actuelles,
            // - remettre target à null.
            uninitMethod.Invoke(instance, null);
            targetField.SetValue(instance, null);

            Log.Info("[UtilitySlots][Quickslots][UI] Forcing quickslot UI reinit.");
        }

        [HarmonyPatch(typeof(uGUI_QuickSlots), "Update")]
        public static class uGUI_QuickSlots_Update_Patch
        {
            static void Prefix(uGUI_QuickSlots __instance)
            {
                if (!_redrawRequested || __instance == null)
                    return;

                _redrawRequested = false;
                ForceReinit(__instance);
            }
        }
    }
}
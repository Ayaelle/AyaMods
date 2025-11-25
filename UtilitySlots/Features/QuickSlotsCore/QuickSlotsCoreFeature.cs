using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;
using UtilitySlots.Features.QuickSlotsUI;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Feature principale QuickSlots+.
    /// Crée un runner MonoBehaviour qui surveille la config et le contexte
    /// (OnFoot/Vehicle) et demande des redessins de l'UI quand nécessaire.
    /// </summary>
    public class QuickSlotsCoreFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            // Si tu as un toggle global, adapte la condition ici.
            if (!RuntimeConfig.EnableInternalAccess)
            {
                Log.Info("[UtilitySlots][Quickslots] QuickSlotsCoreFeature désactivée (config).");
                return;
            }

            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlots_QuickSlotsCoreRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots][Quickslots] Core feature enabled.");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
                Log.Info("[UtilitySlots][Quickslots] Core feature disabled.");
            }
        }

        private class Runner : MonoBehaviour
        {
            private int _lastOnFoot;
            private int _lastVehicle;
            private bool _lastHideEmpty;
            private bool _lastShowLabels;
            private bool _lastInVehicle;

            private void Start()
            {
                _lastOnFoot = RuntimeConfig.OnFootQuickslots;
                _lastVehicle = RuntimeConfig.VehicleQuickslots;
                _lastHideEmpty = RuntimeConfig.HideEmptyQuickSlots;
                _lastShowLabels = RuntimeConfig.ShowQuickSlotLabels;
                _lastInVehicle = IsInVehicle();

                Log.Info(
                    $"[UtilitySlots][Quickslots] Runner started. OnFoot={_lastOnFoot}, Vehicle={_lastVehicle}, InVehicle={_lastInVehicle}"
                );

                // Premier redraw pour s'assurer que l'UI est synchro.
                QuickSlotsUIManager.RequestRedraw();
            }

            private void Update()
            {
                bool changed = false;

                if (!RuntimeConfig.EnableQuickSlots)
                    return;

                if (Player.main == null)
                    return;

                if (_lastOnFoot != RuntimeConfig.OnFootQuickslots)
                {
                    _lastOnFoot = RuntimeConfig.OnFootQuickslots;
                    changed = true;
                }

                if (_lastVehicle != RuntimeConfig.VehicleQuickslots)
                {
                    _lastVehicle = RuntimeConfig.VehicleQuickslots;
                    changed = true;
                }

                if (_lastHideEmpty != RuntimeConfig.HideEmptyQuickSlots)
                {
                    _lastHideEmpty = RuntimeConfig.HideEmptyQuickSlots;
                    changed = true;
                }

                if (_lastShowLabels != RuntimeConfig.ShowQuickSlotLabels)
                {
                    _lastShowLabels = RuntimeConfig.ShowQuickSlotLabels;
                    changed = true;
                }

                bool inVehicle = IsInVehicle();
                if (inVehicle != _lastInVehicle)
                {
                    _lastInVehicle = inVehicle;
                    changed = true;

                    Log.Info(
                        $"[UtilitySlots][Quickslots] Contexte changé : InVehicle={_lastInVehicle}"
                    );
                }

                if (changed)
                {
                    Log.Info(
                        $"[UtilitySlots][Quickslots] Config/Context changed: OnFoot={_lastOnFoot}, Vehicle={_lastVehicle}, HideEmpty={_lastHideEmpty}, ShowLabels={_lastShowLabels}, InVehicle={_lastInVehicle}"
                    );
                    QuickSlotsUIManager.RequestRedraw();
                }
            }

            private static bool IsInVehicle()
            {
                var player = Player.main;
                if (player == null)
                    return false;

                return player.currentMountedVehicle != null;
            }
        }
    }
}

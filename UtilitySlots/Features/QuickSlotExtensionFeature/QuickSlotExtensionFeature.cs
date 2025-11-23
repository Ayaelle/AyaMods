using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickslotExtensionFeature
{
    public class QuickslotExtensionFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlots_QuickslotExtensionRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots][Quickslots] Feature enabled (runner created).");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
                Log.Info("[UtilitySlots][Quickslots] Feature disabled (runner destroyed).");
            }
        }

        private class Runner : MonoBehaviour
        {
            private bool _loggedStart;

            private void Update()
            {
                // On garde juste ces deux gardes de base
                if (!InputManager.Ready)
                    return;

                if (Guard.UIBusy())
                    return;

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;

                string context = vehicle == null ? "OnFoot" : vehicle.GetType().Name;
                int targetSlots = vehicle == null
                    ? RuntimeConfig.OnFootQuickslots
                    : RuntimeConfig.VehicleQuickslots;

                if (!_loggedStart)
                {
                    Log.Info("[UtilitySlots][Quickslots] Runner started.");
                    _loggedStart = true;
                }

                LogDebugContext(context, targetSlots);
            }

            private void LogDebugContext(string context, int targetSlots)
            {
                Log.Info(
                    $"[UtilitySlots][Quickslots] " +
                    $"EnableQuickslotExtension={RuntimeConfig.EnableQuickslotExtension}, " +
                    $"OnFoot={RuntimeConfig.OnFootQuickslots}, " +
                    $"Vehicle={RuntimeConfig.VehicleQuickslots}, " +
                    $"Context={context}, TargetSlots={targetSlots}"
                );
            }
        }
    }
}

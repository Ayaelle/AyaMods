using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;
using UtilitySlots.Features.QuickSlotsUI;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Feature principale QuickSlots+.
    /// Crée un runner MonoBehaviour qui surveille la config runtime
    /// et demande des redessins de l'UI quand nécessaire.
    /// </summary>
    public class QuickSlotsCoreFeature : IFeature
    {
        private static GameObject _runner;

        /// <summary>
        /// Assure qu’un runner existe si QuickSlots est activé.
        /// Peut être appelé depuis n’importe quel patch (ctor QuickSlots, etc.).
        /// </summary>
        public static void EnsureRunner()
        {
            // Si l’objet Unity existe encore, ne rien faire.
            // (Unity surcharge ==, donc un GameObject détruit sera vu comme null ici)
            if (_runner != null)
                return;

            if (!RuntimeConfig.EnableQuickSlots)
            {
                // Inutile de créer le runner si la feature runtime est désactivée.
                return;
            }

            _runner = new GameObject("UtilitySlots_QuickSlotsCoreRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots][Quickslots] Core runner (re)created by EnsureRunner().");
        }

        public void Enable()
        {
            // Correct : on vérifie bien l'option QuickSlots (pas InternalAccess !)
            if (!RuntimeConfig.EnableQuickSlots)
            {
                Log.Info("[UtilitySlots][Quickslots] QuickSlotsCoreFeature désactivée (config).");
                return;
            }

            EnsureRunner();
            Log.Info("[UtilitySlots][Quickslots] Core feature enabled (EnsureRunner called).");
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
            private bool _lastHideEmpty;
            private bool _lastShowLabels;
            private bool _lastInVehicle;
            private bool _loggedDisabled;

            private void Start()
            {
                _lastOnFoot = RuntimeConfig.OnFootQuickslots;
                _lastHideEmpty = RuntimeConfig.HideEmptyQuickSlots;
                _lastShowLabels = RuntimeConfig.ShowQuickSlotLabels;
                _lastInVehicle = IsInVehicle();

                Log.Info(
                    $"[UtilitySlots][Quickslots] Runner started. OnFoot={_lastOnFoot}, InVehicle={_lastInVehicle}"
                );

                // Premier redraw pour s'assurer que l'UI est synchro.
                QuickSlotsUIManager.RequestRedraw();
            }

            private void Update()
            {
                if (!RuntimeConfig.EnableQuickSlots)
                {
                    if (!_loggedDisabled)
                    {
                        Log.Info("[UtilitySlots][Quickslots] Runner disabled by config (EnableQuickSlots = false).");
                        _loggedDisabled = true;
                    }
                    return;
                }

                _loggedDisabled = false;

                if (Player.main == null)
                    return;

                bool changed = false;

                // Vérification : changement du nombre de quickslots (on foot uniquement)
                if (_lastOnFoot != RuntimeConfig.OnFootQuickslots)
                {
                    _lastOnFoot = RuntimeConfig.OnFootQuickslots;
                    changed = true;
                }

                // Hide Empty
                if (_lastHideEmpty != RuntimeConfig.HideEmptyQuickSlots)
                {
                    _lastHideEmpty = RuntimeConfig.HideEmptyQuickSlots;
                    changed = true;
                }

                // Labels
                if (_lastShowLabels != RuntimeConfig.ShowQuickSlotLabels)
                {
                    _lastShowLabels = RuntimeConfig.ShowQuickSlotLabels;
                    changed = true;
                }

                // Changement d'état à pied / en véhicule
                bool inVehicle = IsInVehicle();
                if (inVehicle != _lastInVehicle)
                {
                    _lastInVehicle = inVehicle;
                    changed = true;

                    Log.Info($"[UtilitySlots][Quickslots] Contexte changé : InVehicle={_lastInVehicle}");
                }

                // Action si une option a changé
                if (changed)
                {
                    Log.Info(
                        $"[UtilitySlots][Quickslots] Config/Context changed: OnFoot={_lastOnFoot}, HideEmpty={_lastHideEmpty}, ShowLabels={_lastShowLabels}, InVehicle={_lastInVehicle}"
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
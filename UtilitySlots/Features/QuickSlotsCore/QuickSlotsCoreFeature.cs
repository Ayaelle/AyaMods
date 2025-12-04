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
            private bool _loggedDisabled;

            private void Start()
            {
                _lastOnFoot = RuntimeConfig.OnFootQuickslots;
                _lastHideEmpty = RuntimeConfig.HideEmptyQuickSlots;
                _lastShowLabels = RuntimeConfig.ShowQuickSlotLabels;

                Log.Info(
                    $"[UtilitySlots][Quickslots] Runner started. OnFoot={_lastOnFoot}"
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

                bool changed = false;

                // Changement du nombre de quickslots (OnFoot)
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

                // Action si une option a changé
                if (changed)
                {
                    Log.Info(
                        $"[UtilitySlots][Quickslots] Config changed: OnFoot={_lastOnFoot}, HideEmpty={_lastHideEmpty}, ShowLabels={_lastShowLabels}"
                    );

                    QuickSlotsUIManager.RequestRedraw();
                }
            }
        }
    }
}

using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Feature principale ExtraSlots.
    /// Elle crée un runner pour surveiller les changements
    /// en jeu (puces / modules véhicules) qui nécessitent un redémarrage
    /// ou un relancement de scène.
    /// </summary>
    public class ExtraSlotsFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
            {
                Log.Info("[UtilitySlots][ExtraSlots] ExtraSlots désactivée (config).");
                return;
            }

            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlots_ExtraSlotsRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots][ExtraSlots] ExtraSlots feature enabled.");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
                Log.Info("[UtilitySlots][ExtraSlots] ExtraSlots feature disabled.");
            }
        }

        /// <summary>
        /// Le Runner surveille GLOBAL OPTIONS.
        /// Si les valeurs changent en jeu, on affiche juste un warning
        /// pour dire que ça nécessite un reload de la partie.
        /// </summary>
        private class Runner : MonoBehaviour
        {
            private int _lastPlayerChips;
            private int _lastSeamoth;
            private int _lastExosuit;
            private int _lastCyclops;
            private bool _lastSeamothArm;

            private void Start()
            {
                var g = GlobalOptions.Instance;

                _lastPlayerChips = g.ChipSlots;
                _lastSeamoth = g.SeamothSlots;
                _lastExosuit = g.ExosuitSlots;
                _lastCyclops = g.CyclopsSlots;
                _lastSeamothArm = g.SeamothArmSlots;

                Log.Info("[UtilitySlots][ExtraSlots] Runner started.");
            }

            private void Update()
            {
                var g = GlobalOptions.Instance;
                if (g == null)
                    return;

                bool changed =
                    _lastPlayerChips != g.ChipSlots ||
                    _lastSeamoth != g.SeamothSlots ||
                    _lastExosuit != g.ExosuitSlots ||
                    _lastCyclops != g.CyclopsSlots ||
                    _lastSeamothArm != g.SeamothArmSlots;

                if (changed)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots] Configuration ExtraSlots modifiée.");
                    Log.Warn("Certains changements nécessitent un reload de la partie pour s'appliquer.");

                    _lastPlayerChips = g.ChipSlots;
                    _lastSeamoth = g.SeamothSlots;
                    _lastExosuit = g.ExosuitSlots;
                    _lastCyclops = g.CyclopsSlots;
                    _lastSeamothArm = g.SeamothArmSlots;
                }
            }
        }
    }
}

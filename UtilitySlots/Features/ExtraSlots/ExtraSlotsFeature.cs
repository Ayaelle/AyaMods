using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Feature principale ExtraSlots.
    /// Pour l'instant : extension des slots de puces du joueur uniquement.
    /// Le reste (Seamoth / Exosuit / Cyclops) est désactivé temporairement.
    /// </summary>
    public class ExtraSlotsFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
            {
                Log.Info("[UtilitySlots][ExtraSlots] Disabled by GlobalOptions.");
                return;
            }

            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlots_ExtraSlotsRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots][ExtraSlots] Feature enabled.");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
                Log.Info("[UtilitySlots][ExtraSlots] Feature disabled.");
            }
        }

        private class Runner : MonoBehaviour
        {
            private bool _chipsDone;

            private void Update()
            {
                // On attend que la partie soit chargée et que Inventory.main soit dispo
                if (!_chipsDone && Inventory.main != null && Player.main != null)
                {
                    ExtraSlotsRuntime.EnsurePlayerChipSlots();
                    _chipsDone = true;
                }
            }
        }
    }
}

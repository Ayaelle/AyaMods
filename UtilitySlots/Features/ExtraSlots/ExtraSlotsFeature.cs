using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Feature principale ExtraSlots.
    /// Actuellement : gère UNIQUEMENT les slots de puce du joueur (Chip1..Chip4).
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

            // 1) IMPORTANT : on étend le mapping global une fois
            ExtraSlotsCompatibilityPatches.EnsureGlobalChipSlotMapping();

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

        /// <summary>
        /// Runner MonoBehaviour chargé de détecter Inventory.main / equipment
        /// et d'appeler ExpandChipSlots quand tout est prêt ou que la config change.
        /// </summary>
        private class Runner : MonoBehaviour
        {
            private Equipment _lastEquipment;
            private int _lastAppliedChipSlots;

            private void Update()
            {
                var gopt = GlobalOptions.Instance;
                if (gopt == null || !gopt.EnableExtraSlots)
                    return;

                var inventory = Inventory.main;
                if (inventory == null)
                    return;

                var equipment = inventory.equipment;
                if (equipment == null)
                    return;

                int desired = ExtraSlotsRuntime.GetDesiredChipSlots();

                // Re-apply si :
                // - nouvel objet Equipment (nouvelle scène / reload)
                // - la config ChipSlots a changé
                if (equipment != _lastEquipment || desired != _lastAppliedChipSlots)
                {
                    _lastEquipment = equipment;
                    _lastAppliedChipSlots = desired;

                    // 2) on applique réellement les slots supplémentaires
                    ExtraSlotsPlayerRuntime.ExpandChipSlots(equipment);

                    Log.Info($"[UtilitySlots][ExtraSlots][Player] Applied chip slots: {desired}.");
                }
            }
        }
    }
}
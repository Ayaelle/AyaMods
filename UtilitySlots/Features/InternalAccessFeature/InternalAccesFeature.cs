using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UtilitySlots.Config;
using UnityEngine;

namespace UtilitySlots.Features.InternalAccessFeature
{
    /// <summary>
    /// Gère l'accès améliorations / stockage depuis l'intérieur des véhicules,
    /// en passant proprement par le PDA (pas d'overlay maison).
    /// </summary>
    public class InternalAccessFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            _runner = new GameObject("UtilitySlotsInternalAccessRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();
        }

        public void Disable()
        {
            if (_runner != null)
                Object.Destroy(_runner);
        }

        private class Runner : MonoBehaviour
        {
            private PDA _pda;

            private void Update()
            {
                if (!InputManager.Ready)
                    return;

                if (Guard.UIBusy())
                    return;

                var options = Options.Instance;
                if (options == null || !options.EnableInternalAccess)
                    return;

                var key = options.InternalAccessKey;
                if (!Input.GetKeyDown(key))
                    return;

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;
                if (vehicle == null)
                    return;

                if (vehicle is SeaMoth && !options.SeamothInternalAccess)
                    return;

                if (vehicle is Exosuit && !options.ExosuitInternalAccess)
                    return;

                _pda = player.GetPDA();
                if (_pda == null)
                    return;

                // Ouverture propre du PDA. Ensuite, tu pourras cibler un onglet spécifique si l'API le permet.
                if (!_pda.isOpen)
                    _pda.Open();
            }
        }
    }
}

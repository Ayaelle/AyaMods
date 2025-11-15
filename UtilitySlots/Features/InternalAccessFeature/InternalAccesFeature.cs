using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UtilitySlots.Config;
using UnityEngine;

namespace UtilitySlots.Features.InternalAccessFeature
{
    /// <summary>
    /// Feature qui permet d'ouvrir le PDA depuis l'intérieur d'un véhicule,
    /// via une touche configurable dans le menu Nautilus.
    /// </summary>
    public class InternalAccessFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            // On crée un petit GameObject persistant avec un MonoBehaviour
            // qui écoutera l'input chaque frame.
            _runner = new GameObject("UtilitySlotsInternalAccessRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
            }
        }

        /// <summary>
        /// Composant Unity qui tourne en jeu et gère la touche d'accès interne.
        /// </summary>
        private class Runner : MonoBehaviour
        {
            private PDA _pda;

            private void Update()
            {
                // Si GameInput n'est pas encore prêt, on ne fait rien
                if (!InputManager.Ready)
                    return;

                // Si une UI importante est ouverte (PDA, menu), on ne fait rien
                if (Guard.UIBusy())
                    return;

                var options = Options.Instance;
                if (options == null || !options.EnableInternalAccess)
                    return;

                // Récupère la touche configurée dans les options
                var key = options.InternalAccessKey;
                if (!Input.GetKeyDown(key))
                    return;

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;
                if (vehicle == null)
                    return;

                // Respect des options par type de véhicule
                if (vehicle is SeaMoth && !options.SeamothInternalAccess)
                    return;

                if (vehicle is Exosuit && !options.ExosuitInternalAccess)
                    return;

                // Récupération du PDA du joueur
                _pda = player.GetPDA();
                if (_pda == null)
                    return;

                // Ouverture propre du PDA (c'est lui qui gère l'UI)
                if (!_pda.isOpen)
                    _pda.Open();
            }
        }
    }
}

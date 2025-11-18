using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UtilitySlots.Config;
using UnityEngine;

namespace UtilitySlots.Features.InternalAccessFeature
{

    /// Gère l'accès interne aux upgrades et au stockage depuis l'intérieur
    /// des véhicules, en fonction des options Nautilus.
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

        /// Composant Unity qui tourne en jeu et gère les touches d'accès interne.
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

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;
                if (vehicle == null)
                    return;

                // ----------------------------
                // On ne lit plus UNE seule touche,
                // mais deux : upgrades et stockage.
                // ----------------------------
                bool upgradesPressed = UnityEngine.Input.GetKeyDown(options.InternalUpgradesKey);
                bool storagePressed = UnityEngine.Input.GetKeyDown(options.InternalStorageKey);

                // Si aucune des touches n'est pressée, on quitte.
                if (!upgradesPressed && !storagePressed)
                    return;

                // ----------------------------
                // On route selon le type de véhicule
                // et on délègue à des méthodes dédiées.
                // ----------------------------
                if (vehicle is SeaMoth seamoth)
                {
                    HandleSeamothInternalAccess(seamoth, options, upgradesPressed, storagePressed);
                }
                else if (vehicle is Exosuit exosuit)
                {
                    HandleExosuitInternalAccess(exosuit, options, upgradesPressed, storagePressed);
                }
            }

            // ----------------------------
            // Logique spécifique Seamoth
            // ----------------------------
            private void HandleSeamothInternalAccess(SeaMoth seamoth, Options options, bool upgradesPressed, bool storagePressed)
            {
                // Upgrades depuis l'intérieur du Seamoth (module rack)
                if (upgradesPressed && options.SeamothInternalUpgrades)
                {
                    if (seamoth.upgradesInput != null)
                    {
                        seamoth.upgradesInput.OpenFromExternal();
                        Log.Info("[UtilitySlots] Opened Seamoth upgrade console.");
                    }
                    return;
                }

                // Stockage interne du Seamoth
                if (storagePressed && options.SeamothInternalStorage)
                {
                    try
                    {
                        int slotCount = seamoth.storageInputs?.Length ?? 0;
                        if (slotCount == 0)
                        {
                            Log.Info("[UtilitySlots] Seamoth has no storageInputs.");
                            return;
                        }

                        // Vérifie quels slots contiennent un module de stockage
                        for (int i = 0; i < slotCount; i++)
                        {
                            var tech = seamoth.GetSlotBinding(i);

                            if (tech == TechType.VehicleStorageModule)
                            {
                                var input = seamoth.storageInputs[i];
                                if (input != null)
                                {
                                    input.OpenFromExternal();
                                    Log.Info($"[UtilitySlots] Opened Seamoth storage from slot index {i}.");
                                    return;
                                }
                            }
                        }

                        Log.Info("[UtilitySlots] No VehicleStorageModule installed in Seamoth.");
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error("[UtilitySlots] Error while opening Seamoth storage: " + ex);
                    }
                }
            }

            // ----------------------------
            // Logique spécifique Exosuit
            // ----------------------------
            private void HandleExosuitInternalAccess(Exosuit exosuit, Options options, bool upgradesPressed, bool storagePressed)
            {
                // Upgrades depuis l'intérieur du Prawn
                if (upgradesPressed && options.ExosuitInternalUpgrades)
                {
                    if (exosuit.upgradesInput != null)
                    {
                        exosuit.upgradesInput.OpenFromExternal();
                    }
                    else
                    {
                        OpenPDA();
                    }
                    return;
                }

                // Stockage depuis l'intérieur du Prawn
                if (storagePressed && options.ExosuitInternalStorage)
                {
                    if (exosuit.storageContainer != null)
                    {
                        exosuit.storageContainer.Open();
                    }
                    else
                    {
                        Log.Warn("[UtilitySlots] Exosuit has no storageContainer to open.");
                    }
                    return;
                }
            }

            // Helper pour ouvrir le PDA (utilisé en fallback)
            private void OpenPDA()
            {
                var player = Player.main;
                if (player == null)
                    return;

                _pda = player.GetPDA();
                if (_pda == null)
                    return;

                if (!_pda.isOpen)
                    _pda.Open();
            }
        }
    }
}

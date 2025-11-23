using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.InternalAccessFeature
{
    /// <summary>
    /// Feature qui permet d'ouvrir les upgrades / stockages depuis
    /// l'intérieur des véhicules, en fonction de la config runtime.
    /// </summary>
    public class InternalAccessFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlotsInternalAccessRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots] InternalAccessFeature enabled.");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
            }

            Log.Info("[UtilitySlots] InternalAccessFeature disabled.");
        }

        /// <summary>
        /// Composant Unity qui tourne réellement en jeu et lit la
        /// config runtime à chaque frame.
        /// </summary>
        private class Runner : MonoBehaviour
        {
            private bool _loggedOnce = false;

            private void Update()
            {
                // Log une seule fois pour vérifier que le Runner tourne
                if (!_loggedOnce)
                {
                    Log.Info("[UtilitySlots] InternalAccess Runner.Update() is running.");
                    _loggedOnce = true;
                }

                // 1) Si une UI importante est ouverte (PDA, menu, console…), on ne fait rien.
                if (Guard.UIBusy())
                    return;

                // 2) Récupération directe des options Nautilus
                var opt = RuntimeOptions.Instance;
                if (opt == null || !opt.EnableInternalAccess)
                    return;

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;
                if (vehicle == null)
                    return;

                // 3) Lecture des touches depuis les options
                bool upgradesPressed = global::GameInput.GetButtonDown(Keybinds.InternalUpgrades);
                bool storagePressed = global::GameInput.GetButtonDown(Keybinds.InternalStorage);

                // Si aucune des touches n’est pressée, on quitte.
                if (!upgradesPressed && !storagePressed)
                    return;

                // 4) Dispatch selon le type de véhicule
                if (vehicle is SeaMoth seamoth)
                {
                    HandleSeamothInternalAccess(seamoth, opt, upgradesPressed, storagePressed);
                }
                else if (vehicle is Exosuit exosuit)
                {
                    HandleExosuitInternalAccess(exosuit, opt, upgradesPressed, storagePressed);
                }
            }


            /// <summary>
            /// Gestion de l'accès interne pour le Seamoth.
            /// </summary>
            private void HandleSeamothInternalAccess(SeaMoth seamoth, RuntimeOptions opt, bool upgradesPressed, bool storagePressed)
            {
                if (opt == null)
                    return;

                // Upgrades depuis l'intérieur du Seamoth
                if (upgradesPressed && opt.SeamothInternalUpgrades)
                {
                    if (seamoth.upgradesInput != null)
                    {
                        seamoth.upgradesInput.OpenFromExternal();
                        Log.Info("[UtilitySlots] Opened Seamoth upgrade console.");
                    }
                    return;
                }

                // Stockage interne du Seamoth
                if (storagePressed && opt.SeamothInternalStorage)
                {
                    try
                    {
                        int slotCount = seamoth.storageInputs?.Length ?? 0;
                        if (slotCount == 0)
                        {
                            Log.Info("[UtilitySlots] Seamoth has no storageInputs.");
                            return;
                        }

                        // On vérifie quels slots contiennent un module de stockage
                        for (int i = 0; i < slotCount; i++)
                        {
                            var tech = seamoth.GetSlotBinding(i);

                            if (tech == TechType.VehicleStorageModule)
                            {
                                var input = seamoth.storageInputs[i];
                                if (input != null)
                                {
                                    input.OpenFromExternal();
                                    Log.Info("[UtilitySlots] Opened Seamoth storage (slot " + i + ").");
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

            /// <summary>
            /// Gestion de l'accès interne pour l'Exosuit (Prawn).
            /// </summary>
            private void HandleExosuitInternalAccess(Exosuit exosuit, RuntimeOptions opt, bool upgradesPressed, bool storagePressed)
            {
                if (opt == null)
                    return;

                // Upgrades Prawn
                if (upgradesPressed && opt.ExosuitInternalUpgrades)
                {
                    if (exosuit.upgradesInput != null)
                    {
                        exosuit.upgradesInput.OpenFromExternal();
                        Log.Info("[UtilitySlots] Opened Exosuit upgrade console.");
                    }
                    return;
                }

                // Stockage Prawn
                if (storagePressed && opt.ExosuitInternalStorage)
                {
                    try
                    {
                        var storage = exosuit.storageContainer;
                        if (storage != null)
                        {
                            storage.Open();
                            Log.Info("[UtilitySlots] Opened Exosuit storage.");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error("[UtilitySlots] Error while opening Exosuit storage: " + ex);
                    }
                }
            }

        }
    }
}

using AyaCoreMod.Core;
using AyaCoreMod.Features;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.InternalAccessFeature
{
    public class InternalAccessFeature : IFeature
    {
        private GameObject _runner;

        public void Enable()
        {
            if (_runner != null)
                return;

            _runner = new GameObject("UtilitySlots_InternalAccessRunner");
            Object.DontDestroyOnLoad(_runner);
            _runner.AddComponent<Runner>();

            Log.Info("[UtilitySlots] InternalAccessFeature enabled (runner created).");
        }

        public void Disable()
        {
            if (_runner != null)
            {
                Object.Destroy(_runner);
                _runner = null;
                Log.Info("[UtilitySlots] InternalAccessFeature disabled (runner destroyed).");
            }
        }

        private class Runner : MonoBehaviour
        {
            private bool _loggedOnce;

            private void Update()
            {
                if (!_loggedOnce)
                {
                    Log.Info("[UtilitySlots] InternalAccess Runner.Update() is running.");
                    _loggedOnce = true;
                }

                // UI occupée ? On ne fait rien.
                if (Guard.UIBusy())
                    return;

                var opt = Options.Instance;
                if (opt == null || !opt.EnableInternalAccess)
                    return;

                var player = Player.main;
                if (player == null)
                    return;

                var vehicle = player.currentMountedVehicle;
                if (vehicle == null)
                    return;

                // appels au nouveau système d'input Nautilus
                bool upgradesPressed = global::GameInput.GetButtonDown(Keybinds.InternalUpgrades);
                bool storagePressed = global::GameInput.GetButtonDown(Keybinds.InternalStorage);

                if (!upgradesPressed && !storagePressed)
                    return;

                // DEBUG : log une fois quand on appuie
                Log.Info($"[UtilitySlots] Key pressed. Opts: SeamothUp={opt.SeamothInternalUpgrades}, " +
                         $"SeamothSt={opt.SeamothInternalStorage}, ExoUp={opt.ExosuitInternalUpgrades}, ExoSt={opt.ExosuitInternalStorage}");

                if (vehicle is SeaMoth seamoth)
                {
                    HandleSeamothInternalAccess(seamoth, opt, upgradesPressed, storagePressed);
                }
                else if (vehicle is Exosuit exosuit)
                {
                    HandleExosuitInternalAccess(exosuit, opt, upgradesPressed, storagePressed);
                }
            }

            private static void HandleSeamothInternalAccess(
                SeaMoth seamoth,
                Options opt,
                bool upgradesPressed,
                bool storagePressed)
            {
                // Upgrades internes
                if (upgradesPressed && opt.SeamothInternalUpgrades)
                {
                    if (seamoth.upgradesInput != null)
                    {
                        seamoth.upgradesInput.OpenFromExternal();
                        Log.Info("[UtilitySlots] Opened Seamoth upgrades (internal).");
                    }
                    else
                    {
                        Log.Warn("[UtilitySlots] Seamoth.upgradesInput is null.");
                    }

                    return;
                }

                // Stockage interne Seamoth
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

                        for (int i = 0; i < slotCount; i++)
                        {
                            var tech = seamoth.GetSlotBinding(i);
                            if (tech == TechType.VehicleStorageModule)
                            {
                                var input = seamoth.storageInputs[i];
                                if (input != null)
                                {
                                    input.OpenFromExternal();
                                    Log.Info($"[UtilitySlots] Opened Seamoth storage (slot {i}).");
                                    return;
                                }
                            }
                        }

                        Log.Info("[UtilitySlots] No VehicleStorageModule installed on Seamoth.");
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error("[UtilitySlots] Error while opening Seamoth storage: " + ex);
                    }
                }

                // Si options désactivées, on ne fait rien.
            }

            private static void HandleExosuitInternalAccess(
                Exosuit exosuit,
                Options opt,
                bool upgradesPressed,
                bool storagePressed)
            {
                // Upgrades Prawn
                if (upgradesPressed && opt.ExosuitInternalUpgrades)
                {
                    if (exosuit.upgradesInput != null)
                    {
                        exosuit.upgradesInput.OpenFromExternal();
                        Log.Info("[UtilitySlots] Opened Exosuit upgrades (internal).");
                    }
                    else
                    {
                        Log.Warn("[UtilitySlots] Exosuit.upgradesInput is null.");
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
                            Log.Info("[UtilitySlots] Opened Exosuit storage (internal).");
                        }
                        else
                        {
                            Log.Warn("[UtilitySlots] Exosuit.storageContainer is null.");
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

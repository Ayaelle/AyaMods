using System.Collections;
using UnityEngine;
using Nautilus.Utility;
using UtilitySlots.Config;

namespace UtilitySlots.Features.InternalAccess
{
    /// <summary>
    /// Gère l'ouverture des upgrades ou du stockage depuis l'intérieur des véhicules.
    /// </summary>
    public static class InternalAccessFeature
    {
        public static bool Enabled => Options.Instance.EnableInternalAccess;

        public static void Update()
        {
            if (!Enabled)
                return;

            var options = Options.Instance;

            // Raccourci d'accès
            var key = options.InternalAccessKey;
            if (!Input.GetKeyDown(key))
                return;

            // Si le joueur n'est pas dans un véhicule, on ne fait rien
            var vehicle = Player.main.currentMountedVehicle;
            if (!vehicle)
                return;

            // Anti-overlay : si une autre UI est ouverte => ne rien faire
            if (PDA.isOpen) return;
            if (IngameMenu.main && IngameMenu.main.isActiveAndEnabled) return;
            if (uGUI.main?.loading != null && uGUI.main.loading.activeSelf) return;
            if (DevConsole.instance && DevConsole.instance.state == DevConsoleState.Open) return;

            // Si Seamoth
            if (vehicle is SeaMoth seamoth)
            {
                seamoth.upgradesInput.OpenFromExternal();
                return;
            }

            // Si Prawn
            if (vehicle is Exosuit prawn)
            {
                prawn.storageContainer?.Open();
                return;
            }
        }
    }
}

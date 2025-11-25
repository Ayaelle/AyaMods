using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsCore
{
    public enum QuickSlotsContext
    {
        OnFoot,
        Vehicle
    }

    /// <summary>
    /// Logique de calcul des nombres de slots selon le contexte et la config.
    /// </summary>
    public static class QuickSlotsRuntime
    {
        public const int HardMaxSlots = 12;

        /// <summary>
        /// Retourne le contexte courant (à pied / en véhicule) d'après le Player.
        /// </summary>
        public static QuickSlotsContext GetCurrentContext()
        {
            Player player = Player.main;
            if (player != null && player.GetMode() == Player.Mode.Piloting)
            {
                // Pilotage Seamoth / Exosuit / etc.
                return QuickSlotsContext.Vehicle;
            }

            return QuickSlotsContext.OnFoot;
        }

        /// <summary>
        /// Nombre de slots souhaités pour un contexte donné, limité au hard cap.
        /// </summary>
        public static int GetConfiguredSlots(QuickSlotsContext context)
        {
            // Si la feature runtime est désactivée, on retombe sur le comportement vanilla.
            if (!RuntimeConfig.EnableQuickSlots)
                return 5; // valeur vanilla

            int requested = context == QuickSlotsContext.Vehicle
                ? RuntimeConfig.VehicleQuickslots
                : RuntimeConfig.OnFootQuickslots;

            return Mathf.Clamp(requested, 1, HardMaxSlots);
        }

        /// <summary>
        /// Nombre de slots "physiques" maximum à créer dans QuickSlots.
        /// </summary>
        public static int GetPhysicalSlots()
        {
            // On prend le max de slot quoi qu'il arrive puis on change le nombre qu'on
            // expose réellement via GetSlotCount en fonction du contexte.
            return HardMaxSlots;
        }
    }
}

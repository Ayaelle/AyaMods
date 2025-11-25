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
        /// Pour l'instant on ne gère que le contexte "à pied".
        /// Les véhicules resteront vanilla jusqu'à ce qu'on les branche proprement.
        /// </summary>
        public static QuickSlotsContext GetCurrentContext()
        {
            return QuickSlotsContext.OnFoot;
        }

        /// <summary>
        /// Nombre de slots configuré pour le contexte donné.
        /// (Actuellement le contexte sera toujours OnFoot.)
        /// </summary>
        public static int GetConfiguredSlots(QuickSlotsContext context)
        {
            int requested = context == QuickSlotsContext.OnFoot
                ? RuntimeConfig.OnFootQuickslots
                : RuntimeConfig.VehicleQuickslots;

            return Mathf.Clamp(requested, 1, HardMaxSlots);
        }

        /// <summary>
        /// Nombre de slots "physiques" maximum à créer dans QuickSlots.
        /// </summary>
        public static int GetPhysicalSlots()
        {
            // On prend le max de slots quoi qu'il arrive, puis on limite le nombre utilisé via GetSlotCount.
            return HardMaxSlots;
        }
    }
}

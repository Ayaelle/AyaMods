using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Petit helper centralisé pour lire GlobalOptions et
    /// calculer proprement le nombre de slots désirés.
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        // Hard caps raisonnables : on reste large sans exploser l'UI.
        public const int MaxPlayerChips = 6;
        public const int MaxSeamothSlots = 12;
        public const int MaxExosuitSlots = 12;
        public const int MaxCyclopsSlots = 14;

        /// <summary>
        /// Nombre de slots puce joueur demandé en config globale.
        /// </summary>
        public static int GetPlayerChipSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null)
                return 4; // vanilla

            return Mathf.Clamp(gopt.ChipSlots, 2, MaxPlayerChips);
        }

        /// <summary>
        /// Nombre de slots modules Seamoth demandé en config globale.
        /// </summary>
        public static int GetSeamothModuleSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null)
                return 4; // vanilla

            return Mathf.Clamp(gopt.SeamothSlots, 4, MaxSeamothSlots);
        }

        /// <summary>
        /// Nombre de slots modules Exosuit demandé en config globale.
        /// </summary>
        public static int GetExosuitModuleSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null)
                return 4; // vanilla

            return Mathf.Clamp(gopt.ExosuitSlots, 4, MaxExosuitSlots);
        }

        /// <summary>
        /// Nombre de slots modules Cyclops demandé en config globale.
        /// </summary>
        public static int GetCyclopsModuleSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null)
                return 6; // vanilla

            return Mathf.Clamp(gopt.CyclopsSlots, 6, MaxCyclopsSlots);
        }
    }
}

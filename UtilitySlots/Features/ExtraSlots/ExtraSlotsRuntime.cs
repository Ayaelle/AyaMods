using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helpers pour ExtraSlots (ex : chip slots joueur).
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        // Vanilla Subnautica
        public const int VanillaChipSlots = 2;

        // Ce qu'on autorisera quand l’UI sera 100% étendue
        public const int MinChipSlots = 2;
        public const int MaxChipSlots = 6;

        /// <summary>
        /// ExtraSlots activé dans les options ?
        /// </summary>
        public static bool IsEnabled()
        {
            var gopt = GlobalOptions.Instance;
            return gopt != null && gopt.EnableExtraSlots;
        }

        /// <summary>
        /// Nombre demandé par le joueur (Option), clampé entre Min et Max.
        /// </summary>
        public static int GetDesiredChipSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return VanillaChipSlots;

            int requested = gopt.ChipSlots;

            if (requested < MinChipSlots)
                requested = MinChipSlots;
            if (requested > MaxChipSlots)
                requested = MaxChipSlots;

            return requested;
        }

        /// <summary>
        /// Alias utilisé par les patches UI.
        /// </summary>
        public static int GetDesiredPlayerChips()
        {
            return GetDesiredChipSlots();
        }
    }
}

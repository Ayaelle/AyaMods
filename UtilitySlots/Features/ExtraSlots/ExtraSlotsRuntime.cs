using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helpers pour ExtraSlots (slots de puces joueur).
    /// Ne gère que la config globale / clamps.
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        /// <summary>Nombre de slots vanilla.</summary>
        public const int VanillaChipSlots = 2;

        /// <summary>
        /// Hard cap actuel pour l'UI (on gère 4 puces maximum).
        /// </summary>
        public const int MaxChipSlots = 4;

        /// <summary>
        /// ExtraSlots activé dans GlobalOptions ?
        /// </summary>
        public static bool IsEnabled()
        {
            var g = GlobalOptions.Instance;
            return g != null && g.EnableExtraSlots;
        }

        /// <summary>
        /// Nombre de slots demandé par la config, clampé.
        /// </summary>
        public static int GetDesiredChipSlots()
        {
            var g = GlobalOptions.Instance;
            if (g == null || !g.EnableExtraSlots)
                return VanillaChipSlots;

            int requested = g.ChipSlots;
            if (requested < VanillaChipSlots)
                requested = VanillaChipSlots;
            if (requested > MaxChipSlots)
                requested = MaxChipSlots;

            return requested;
        }

        /// <summary>
        /// Alias utilisé par certains patches UI (compat historique).
        /// </summary>
        public static int GetDesiredPlayerChips() => GetDesiredChipSlots();
    }
}

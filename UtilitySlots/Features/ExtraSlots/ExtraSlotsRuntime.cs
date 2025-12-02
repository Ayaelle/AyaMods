using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helpers pour ExtraSlots (principalement les slots de puces joueur).
    /// Source de vérité pour le nombre de chip slots.
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        /// <summary> Nombre de slots vanilla. </summary>
        public const int VanillaChipSlots = 2;

        /// <summary>
        /// Pour l’instant on ne gère l’UI que jusqu’à 4 slots.
        /// Si plus tard on veut aller jusque 6, il faudra aussi étendre l’UI.
        /// </summary>
        public const int MaxChipSlots = 4;

        /// <summary>
        /// True si ExtraSlots est activé dans les options globales.
        /// </summary>
        public static bool IsEnabled()
        {
            var gopt = GlobalOptions.Instance;
            return gopt != null && gopt.EnableExtraSlots;
        }

        /// <summary>
        /// Nombre de chip slots demandés par la config, clampé entre VanillaChipSlots et MaxChipSlots.
        /// </summary>
        public static int GetDesiredChipSlots()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return VanillaChipSlots;

            int requested = gopt.ChipSlots;

            if (requested < VanillaChipSlots)
                requested = VanillaChipSlots;

            if (requested > MaxChipSlots)
                requested = MaxChipSlots;

            return requested;
        }

        /// <summary>
        /// Alias pour compat avec certains patches UI.
        /// </summary>
        public static int GetDesiredPlayerChips()
        {
            return GetDesiredChipSlots();
        }
    }
}

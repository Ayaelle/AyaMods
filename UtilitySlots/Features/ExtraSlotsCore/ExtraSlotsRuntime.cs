using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlotsCore
{
    /// <summary>
    /// Runtime helpers pour ExtraSlotsCore (principalement les slots de puces joueur).
    /// Source de vérité pour le nombre de chip slots.
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        /// <summary> Nombre de slots vanilla. </summary>
        public const int VanillaChipSlots = 2;

        /// <summary> Minimum configurable (au moins vanilla). </summary>
        public const int MinChipSlots = VanillaChipSlots;

        /// <summary> Borne max physique que l’on gère. </summary>
        public const int MaxChipSlots = 6;

        /// <summary> True si ExtraSlotsCore est activé dans les options globales. </summary>
        public static bool IsEnabled()
        {
            var gopt = GlobalOptions.Instance;
            return gopt != null && gopt.EnableExtraSlots;
        }

        /// <summary>
        /// Nombre de chip slots "désirés" par la config, clampé entre MinChipSlots et MaxChipSlots.
        /// C'est le nombre de slots réellement utilisés/affichés.
        /// </summary>
        public static int GetDesiredChipSlots()
        {
            if (!IsEnabled())
                return VanillaChipSlots;

            var gopt = GlobalOptions.Instance;
            int requested = gopt != null ? gopt.ChipSlots : VanillaChipSlots;

            if (requested < MinChipSlots)
                requested = MinChipSlots;

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

using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlotsVehciles
{
    /// <summary>
    /// Helpers runtime pour les slots de modules véhicules (Seamoth / Exosuit / Cyclops).
    /// Ne gère que la partie "nombre de slots" côté logique, pas l'UI.
    /// </summary>
    public static class ExtraSlotsVehiclesRuntime
    {
        // Vanilla
        public const int VanillaSeamothModuleSlots = 4;
        public const int VanillaExosuitModuleSlots = 4;
        public const int VanillaCyclopsModuleSlots = 6;

        // Cibles max
        public const int MaxSeamothModuleSlots = 12;
        public const int MaxExosuitModuleSlots = 12;
        public const int MaxCyclopsModuleSlots = 14;

        private static GlobalOptions GOpt => GlobalOptions.Instance;

        private static bool ExtraSlotsEnabled()
        {
            var g = GOpt;
            return g != null && g.EnableExtraSlots;
        }

        /// <summary>
        /// Nombre de slots de modules souhaité pour le Seamoth.
        /// Clampé entre vanilla et max.
        /// </summary>
        public static int GetDesiredSeamothModuleSlots()
        {
            if (!ExtraSlotsEnabled())
                return VanillaSeamothModuleSlots;

            var g = GOpt;
            int requested = g != null ? g.SeamothModuleSlots : VanillaSeamothModuleSlots;

            if (requested < VanillaSeamothModuleSlots)
                requested = VanillaSeamothModuleSlots;

            if (requested > MaxSeamothModuleSlots)
                requested = MaxSeamothModuleSlots;

            return requested;
        }

        /// <summary>
        /// Nombre de slots de modules souhaité pour l’Exosuit (hors bras).
        /// Clampé entre vanilla et max.
        /// </summary>
        public static int GetDesiredExosuitModuleSlots()
        {
            if (!ExtraSlotsEnabled())
                return VanillaExosuitModuleSlots;

            var g = GOpt;
            int requested = g != null ? g.ExosuitModuleSlots : VanillaExosuitModuleSlots;

            if (requested < VanillaExosuitModuleSlots)
                requested = VanillaExosuitModuleSlots;

            if (requested > MaxExosuitModuleSlots)
                requested = MaxExosuitModuleSlots;

            return requested;
        }

        /// <summary>
        /// Nombre de slots de modules souhaité pour le Cyclops.
        /// Clampé entre vanilla et max.
        /// </summary>
        public static int GetDesiredCyclopsModuleSlots()
        {
            if (!ExtraSlotsEnabled())
                return VanillaCyclopsModuleSlots;

            var g = GOpt;
            int requested = g != null ? g.CyclopsModuleSlots : VanillaCyclopsModuleSlots;

            if (requested < VanillaCyclopsModuleSlots)
                requested = VanillaCyclopsModuleSlots;

            if (requested > MaxCyclopsModuleSlots)
                requested = MaxCyclopsModuleSlots;

            return requested;
        }

        /// <summary>
        /// Construit la liste complète des slotIDs pour le Seamoth :
        /// "SeamothModule1".."SeamothModuleN".
        /// </summary>
        public static string[] BuildSeamothSlotIDs(int desired)
        {
            if (desired <= 0)
                return new string[0];

            var arr = new string[desired];
            for (int i = 0; i < desired; i++)
                arr[i] = "SeamothModule" + (i + 1);

            return arr;
        }

        /// <summary>
        /// Construit la liste complète des slotIDs pour l’Exosuit :
        /// bras + "ExosuitModule1".."ExosuitModuleN".
        /// L’ordre reste : bras gauche, bras droit, puis modules.
        /// </summary>
        public static string[] BuildExosuitSlotIDs(int desiredModules)
        {
            if (desiredModules < VanillaExosuitModuleSlots)
                desiredModules = VanillaExosuitModuleSlots;

            // 2 bras + N modules
            var arr = new string[2 + desiredModules];

            arr[0] = "ExosuitArmLeft";
            arr[1] = "ExosuitArmRight";

            for (int i = 0; i < desiredModules; i++)
                arr[2 + i] = "ExosuitModule" + (i + 1);

            return arr;
        }
    }
}

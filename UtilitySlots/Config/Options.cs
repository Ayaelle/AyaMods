using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options Nautilus pour UtilitySlots.
    /// Permet de configurer le nombre de slots/chips/quickslots et certaines features.
    /// </summary>
    [Menu("UtilitySlots Settings")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; private set; }

        public Options() : base("UtilitySlotsOptions")
        {
            Instance = this;
        }

        // --- Chips joueur ---

        [Slider("Player chip slots", 2, 6)]
        public int ChipSlots { get; set; } = 4;

        // --- Seamoth ---

        [Slider("Seamoth module slots", 4, 12)]
        public int SeamothSlots { get; set; } = 8;

        [Toggle("Internal access (Seamoth)")]
        public bool SeamothInternalAccess { get; set; } = true;

        [Toggle("Enable Seamoth arm slots")]
        public bool SeamothArmSlots { get; set; } = true;


        // --- Prawn (Exosuit) ---

        [Slider("Prawn module slots", 4, 12)]
        public int ExosuitSlots { get; set; } = 8;

        [Toggle("Internal access (Prawn)")]
        public bool ExosuitInternalAccess { get; set; } = true;

        // --- Cyclops ---

        [Slider("Cyclops module slots", 6, 14)]
        public int CyclopsSlots { get; set; } = 10;

        // --- Quickslots ---

        [Toggle("Enable quickslot extension")]
        public bool EnableQuickslotExtension { get; set; } = true;

        [Slider("On-foot quickslots", 4, 12)]
        public int OnFootQuickslots { get; set; } = 8;

        [Slider("In-vehicle quickslots", 4, 12)]
        public int VehicleQuickslots { get; set; } = 8;

        // --- Internal Access général ---

        [Toggle("Enable internal access feature")]
        public bool EnableInternalAccess { get; set; } = true;

        [Keybind("Open upgrades/storage")]
        public KeyCode InternalAccessKey { get; set; } = KeyCode.U;
    }
}

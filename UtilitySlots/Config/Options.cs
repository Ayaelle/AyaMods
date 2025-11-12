using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace AyaCoreMod.UtilitySlots.Config
{
    [Menu("AyaMods: Utility Slots")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; private set; }
        public Options() : base("AyaMods_UtilitySlots_Options") { Instance = this; }

        [Header("Chips (Player)")]
        [Slider("Player chip slots", 2, 6)]
        public int ChipSlots { get; set; } = 4;

        [Header("Seamoth")]
        [Slider("Module slots", 4, 12)] public int SeamothSlots { get; set; } = 8;
        [Toggle("Internal access (Seamoth)")] public bool SeamothInternal { get; set; } = true;

        [Header("Prawn (Exosuit)")]
        [Slider("Module slots", 4, 12)] public int ExosuitSlots { get; set; } = 8;
        [Toggle("Internal access (Prawn)")] public bool ExosuitInternal { get; set; } = true;

        [Header("Cyclops")]
        [Slider("Module slots", 6, 14)] public int CyclopsSlots { get; set; } = 10;

        [Header("Quickslots")]
        [Toggle("Enable quickslot extension")] public bool EnableQuickslotExtension { get; set; } = true;
        [Slider("On-foot quickslots", 4, 12)] public int OnFootQuickslots { get; set; } = 8;
        [Slider("In-vehicle quickslots", 4, 12)] public int VehicleQuickslots { get; set; } = 8;

        [Header("Internal Access")]
        [Toggle("Enable internal access feature")] public bool EnableInternalAccess { get; set; } = true;
        [Keybind("Open upgrades/storage")] public KeyCode InternalAccessKey { get; set; } = KeyCode.U;
    }
}

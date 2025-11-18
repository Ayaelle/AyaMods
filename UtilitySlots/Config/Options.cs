using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UtilitySlots.Config
{

    /// Options "live" devant modifier le jeu à la volée

    [Menu("Utility Slots (In-Game)")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; private set; }

        public Options() : base("UtilitySlotsRuntime")
        {
            Instance = this;
        }

        // -----------------------------
        // INTERNAL ACCESS (LIVE-NOT WORKING UPDATE DONT GET CALLED)
        // -----------------------------

        [Toggle("Enable all internal access")]
        public bool EnableInternalAccess { get; set; } = true;

        [Toggle("Seamoth internal upgrades access")]
        public bool SeamothInternalUpgrades { get; set; } = true;

        [Toggle("Seamoth internal storages access")]
        public bool SeamothInternalStorage { get; set; } = true;

        [Toggle("Prawn internal upgrades access")]
        public bool ExosuitInternalUpgrades { get; set; } = true;

        [Toggle("Prawn internal storage access")]
        public bool ExosuitInternalStorage { get; set; } = true;

        [Keybind("Internal upgrades key")]
        public KeyCode InternalUpgradesKey = KeyCode.U;

        [Keybind("Internal storage key")]
        public KeyCode InternalStorageKey = KeyCode.I;
    }

    /// Options "globales" qui contrôlent la disposition des slots, quickslots, etc.
    /// Les changements ici nécessitent un reload de la partie (ou du jeu) pour
    /// être pris en compte.
   
    [Menu("Utility Slots (Global - restart required)")]
    public class GlobalOptions : ConfigFile
    {
        public static GlobalOptions Instance { get; private set; }

        public GlobalOptions() : base("UtilitySlotsGlobal")
        {
            Instance = this;
        }

        // -----------------------------
        // TOGGLE GENERAL
        // -----------------------------

        [Toggle("Enable extra slots mod")]
        public bool EnableExtraSlots { get; set; } = true;

        // -----------------------------
        // PLAYER CHIPS
        // -----------------------------

        [Slider("Player chip slots", 2, 6, DefaultValue = 4)]
        public int ChipSlots = 4;

        // -----------------------------
        // SEAMOTH
        // -----------------------------

        [Slider("Seamoth module slots", 4, 12, DefaultValue = 12)]
        public int SeamothSlots = 12;

        [Toggle("Enable Seamoth arms")]
        public bool SeamothArmSlots { get; set; } = true;

        // -----------------------------
        // PRAWN
        // -----------------------------

        [Slider("Prawn module slots", 4, 12, DefaultValue = 12)]
        public int ExosuitSlots = 12;

        // -----------------------------
        // CYCLOPS
        // -----------------------------

        [Slider("Cyclops module slots", 6, 14, DefaultValue = 14)]
        public int CyclopsSlots = 14;

        // -----------------------------
        // QUICKLOTS
        // -----------------------------

        [Toggle("Enable extra quickslots")]
        public bool EnableQuickslotExtension { get; set; } = true;

        [Slider("On-foot quickslots", 4, 12, DefaultValue = 12)]
        public int OnFootQuickslots = 12;

        [Slider("In-vehicle quickslots", 4, 12, DefaultValue = 12)]
        public int VehicleQuickslots = 12;
    }
}

using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options "runtime" d'UtilitySlots, visibles dans Nautilus et
    /// modifiables en jeu. Les setters poussent leurs valeurs dans
    /// RuntimeInternalAccessConfig pour un comportement live.
    /// </summary>
    [Menu("Utility Slots (In-Game)")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; private set; }

        public Options() : base("UtilitySlotsRuntime")
        {
            Instance = this;

            // Au chargement initial (lecture du JSON par Nautilus),
            // on s'assure que la config runtime reflète bien les valeurs.
            RuntimeInternalAccessConfig.ApplyFrom(this);
        }

        // ------------------------
        // TOGGLES GLOBAUX
        // ------------------------

        private bool _enableInternalAccess = true;

        [Toggle("Enable all internal access")]
        public bool EnableInternalAccess
        {
            get => _enableInternalAccess;
            set
            {
                _enableInternalAccess = value;
                RuntimeInternalAccessConfig.EnableInternalAccess = value;
            }
        }

        // ------------------------
        // SEAMOTH
        // ------------------------

        private bool _seamothInternalUpgrades = true;

        [Toggle("Seamoth internal upgrades access")]
        public bool SeamothInternalUpgrades
        {
            get => _seamothInternalUpgrades;
            set
            {
                _seamothInternalUpgrades = value;
                RuntimeInternalAccessConfig.SeamothInternalUpgrades = value;
            }
        }

        private bool _seamothInternalStorage = true;

        [Toggle("Seamoth internal storages access")]
        public bool SeamothInternalStorage
        {
            get => _seamothInternalStorage;
            set
            {
                _seamothInternalStorage = value;
                RuntimeInternalAccessConfig.SeamothInternalStorage = value;
            }
        }

        // ------------------------
        // EXOSUIT (PRAWN)
        // ------------------------

        private bool _exosuitInternalUpgrades = true;

        [Toggle("Prawn internal upgrades access")]
        public bool ExosuitInternalUpgrades
        {
            get => _exosuitInternalUpgrades;
            set
            {
                _exosuitInternalUpgrades = value;
                RuntimeInternalAccessConfig.ExosuitInternalUpgrades = value;
            }
        }

        private bool _exosuitInternalStorage = true;

        [Toggle("Prawn internal storage access")]
        public bool ExosuitInternalStorage
        {
            get => _exosuitInternalStorage;
            set
            {
                _exosuitInternalStorage = value;
                RuntimeInternalAccessConfig.ExosuitInternalStorage = value;
            }
        }

        // ------------------------
        // KEYBINDS
        // ------------------------

        private KeyCode _internalUpgradesKey = KeyCode.U;

        [Keybind("Internal upgrades key")]
        public KeyCode InternalUpgradesKey
        {
            get => _internalUpgradesKey;
            set
            {
                _internalUpgradesKey = value;
                RuntimeInternalAccessConfig.InternalUpgradesKey = value;
            }
        }

        private KeyCode _internalStorageKey = KeyCode.I;

        [Keybind("Internal storage key")]
        public KeyCode InternalStorageKey
        {
            get => _internalStorageKey;
            set
            {
                _internalStorageKey = value;
                RuntimeInternalAccessConfig.InternalStorageKey = value;
            }
        }
    }

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

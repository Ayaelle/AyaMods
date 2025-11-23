using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Configuration runtime réelle utilisée par le mod en jeu.
    /// C'est cette classe que les features lisent directement.
    /// </summary>
    public static class RuntimeConfig
    {
        public static bool EnableInternalAccess = true;
        public static bool SeamothInternalUpgrades = true;
        public static bool SeamothInternalStorage = true;
        public static bool ExosuitInternalUpgrades = true;
        public static bool ExosuitInternalStorage = true;
    }

    /// <summary>
    /// Options modifiables en jeu via le menu Nautilus.
    /// Les setters mettent à jour RuntimeConfig.
    /// </summary>
    [Menu("Utility Slots (In-Game)")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; internal set; }

        public Options() : base("UtilitySlotsRuntime")
        {
            Instance = this;
        }

        // -----------------------------
        // INTERNAL ACCESS
        // -----------------------------

        [Toggle("Enable all internal access")]
        public bool EnableInternalAccess
        {
            get => RuntimeConfig.EnableInternalAccess;
            set => RuntimeConfig.EnableInternalAccess = value;
        }

        [Toggle("Seamoth internal upgrades access")]
        public bool SeamothInternalUpgrades
        {
            get => RuntimeConfig.SeamothInternalUpgrades;
            set => RuntimeConfig.SeamothInternalUpgrades = value;
        }

        [Toggle("Seamoth internal storage access")]
        public bool SeamothInternalStorage
        {
            get => RuntimeConfig.SeamothInternalStorage;
            set => RuntimeConfig.SeamothInternalStorage = value;
        }

        [Toggle("Prawn internal upgrades access")]
        public bool ExosuitInternalUpgrades
        {
            get => RuntimeConfig.ExosuitInternalUpgrades;
            set => RuntimeConfig.ExosuitInternalUpgrades = value;
        }

        [Toggle("Prawn internal storage access")]
        public bool ExosuitInternalStorage
        {
            get => RuntimeConfig.ExosuitInternalStorage;
            set => RuntimeConfig.ExosuitInternalStorage = value;
        }
    }

    /// <summary>
    /// Options globales (slots, quickslots, etc) – reboot requis.
    /// </summary>
    [Menu("Utility Slots (Global - restart required)")]
    public class GlobalOptions : ConfigFile
    {
        public static GlobalOptions Instance { get; internal set; }

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
        public int ChipSlots { get; set; } = 4;

        // -----------------------------
        // SEAMOTH
        // -----------------------------

        [Slider("Seamoth module slots", 4, 12, DefaultValue = 12)]
        public int SeamothSlots { get; set; } = 12;

        [Toggle("Enable Seamoth arms")]
        public bool SeamothArmSlots { get; set; } = true;

        // -----------------------------
        // PRAWN
        // -----------------------------

        [Slider("Prawn module slots", 4, 12, DefaultValue = 12)]
        public int ExosuitSlots { get; set; } = 12;

        // -----------------------------
        // CYCLOPS
        // -----------------------------

        [Slider("Cyclops module slots", 6, 14, DefaultValue = 14)]
        public int CyclopsSlots { get; set; } = 14;

        // -----------------------------
        // QUICKLOTS
        // -----------------------------

        [Toggle("Enable extra quickslots")]
        public bool EnableQuickslotExtension { get; set; } = true;

        [Slider("On-foot quickslots", 4, 12, DefaultValue = 12)]
        public int OnFootQuickslots { get; set; } = 12;

        [Slider("In-vehicle quickslots", 4, 12, DefaultValue = 12)]
        public int VehicleQuickslots { get; set; } = 12;
    }
}

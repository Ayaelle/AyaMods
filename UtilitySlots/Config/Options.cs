using Nautilus.Json;
using Nautilus.Options.Attributes;
using System.Net;

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

        // QUICKSLOTS
        public static bool EnableQuickSlots = true;
        public static bool HideEmptyQuickSlots = false;
        public static bool ShowQuickSlotLabels = true;
        public static int OnFootQuickslots = 12;

        // EXTRASLOTS
        public static bool EnableExtraSlots = true;
        public static int ChipSlots = 4;
        public static int SeamothModuleSlots = 12;
        public static bool SeamothArmSlots = true;
        public static int ExosuitModuleSlots = 12;
        public static int CyclopsSlots = 14;
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

        // -----------------------------
        // QUICKSLOTS
        // -----------------------------

        [Toggle("Hide Empty Slots")]
        public bool HideEmptyQuickSlots
        {
            get => RuntimeConfig.HideEmptyQuickSlots;
            set => RuntimeConfig.HideEmptyQuickSlots = value;
        }

        [Toggle("Show Quickslots labels")]
        public bool ShowQuickSlotLabels
        {
            get => RuntimeConfig.ShowQuickSlotLabels;
            set => RuntimeConfig.ShowQuickSlotLabels = value;
        }

        [Slider("Quickslots", 4, 12, DefaultValue = 12)]
        public int OnFootQuickslots
        {
            get => RuntimeConfig.OnFootQuickslots;
            set => RuntimeConfig.OnFootQuickslots = value;
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
        public bool EnableExtraSlots
        {
            get => RuntimeConfig.EnableExtraSlots;
            set => RuntimeConfig.EnableExtraSlots = value;
        }

        // -----------------------------
        // PLAYER CHIPS
        // -----------------------------

        [Slider("Player chip slots", 2, 6, DefaultValue = 4)]
        public int ChipSlots
        {
            get => RuntimeConfig.ChipSlots;
            set => RuntimeConfig.ChipSlots = value;
        }

        // -----------------------------
        // SEAMOTH
        // -----------------------------

        [Slider("Seamoth module slots", 4, 12, DefaultValue = 12)]
        public int SeamothSlots
        {
            get => RuntimeConfig.SeamothModuleSlots;
            set => RuntimeConfig.SeamothModuleSlots = value;
        }

        [Toggle("Enable Seamoth arms")]
        public bool SeamothArmSlots
        {
            get => RuntimeConfig.SeamothArmSlots;
            set => RuntimeConfig.SeamothArmSlots = value;
        }

        // -----------------------------
        // PRAWN
        // -----------------------------

        [Slider("Prawn module slots", 4, 12, DefaultValue = 12)]
        public int ExosuitSlots
        {
            get => RuntimeConfig.ExosuitModuleSlots;
            set => RuntimeConfig.ExosuitModuleSlots = value;
        }

        // -----------------------------
        // CYCLOPS
        // -----------------------------

        [Slider("Cyclops module slots", 6, 14, DefaultValue = 14)]
        public int CyclopsSlots
        {
            get => RuntimeConfig.CyclopsSlots;
            set => RuntimeConfig.CyclopsSlots = value;
        }

        // -----------------------------
        // QUICKSLOTS
        // -----------------------------

        [Toggle("Enable quickslots mod")]
        public bool EnableQuickSlots
        {
            get => RuntimeConfig.EnableQuickSlots;
            set => RuntimeConfig.EnableQuickSlots = value;
        }
    }
}

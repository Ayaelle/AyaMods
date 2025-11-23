using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options globales (changement nécessite un redémarrage de la partie / du jeu).
    /// Utilisées pour configurer les nombres de slots, quickslots, etc.
    /// </summary>
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
        public int ChipSlots { get; set; } = 4;

        // -----------------------------
        // SEAMOTH
        // -----------------------------

        [Slider("Seamoth module slots", 4, 12, DefaultValue = 12)]
        public int SeamothSlots { get; set; } = 12;

        [Toggle("Enable Seamoth arms")]
        public bool SeamothArmSlots { get; set; } = true;

        // -----------------------------
        // PRAWN / EXOSUIT
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

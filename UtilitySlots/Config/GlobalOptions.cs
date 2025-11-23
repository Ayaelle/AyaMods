using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;

namespace UtilitySlots.Config
{
    [Menu("Utility Slots (Global - restart required)")]
    public class GlobalOptions : ConfigFile
    {
        public static GlobalOptions Instance { get; private set; }

        public GlobalOptions() : base("UtilitySlotsGlobal")
        {
            Instance = this;
        }

        // -------- EXTRA SLOTS --------

        [Toggle("Enable extra slots system")]
        public bool EnableExtraSlots { get; set; } = true;

        [Slider("Player chip slots", 2, 6, DefaultValue = 4)]
        public int ChipSlots = 4;

        [Slider("Seamoth module slots", 4, 12, DefaultValue = 12)]
        public int SeamothSlots = 12;

        [Toggle("Enable Seamoth arms")]
        public bool SeamothArmSlots { get; set; } = true;

        [Slider("Exosuit module slots", 4, 12, DefaultValue = 12)]
        public int ExosuitSlots = 12;

        [Slider("Cyclops module slots", 6, 14, DefaultValue = 14)]
        public int CyclopsSlots = 14;

        // -------- QUICK SLOTS --------

        [Toggle("Enable quickslot extension")]
        public bool EnableQuickslotExtension { get; set; } = true;

        [Slider("On-foot quickslots", 4, 12, DefaultValue = 12)]
        public int OnFootQuickslots = 12;

        [Slider("In-vehicle quickslots", 4, 12, DefaultValue = 12)]
        public int VehicleQuickslots = 12;
    }
}

using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options modifiables en jeu (runtime) pour UtilitySlots.
    /// Utilisées pour piloter l'accès interne aux upgrades / stockage.
    /// </summary>
    [Menu("Utility Slots (In-Game)")]
    public class Options : ConfigFile
    {
        public static Options Instance { get; private set; }

        public Options() : base("UtilitySlotsRuntime")
        {
            Instance = this;
        }

        // -----------------------------
        // INTERNAL ACCESS GLOBAL
        // -----------------------------

        [Toggle("Enable all internal access")]
        public bool EnableInternalAccess { get; set; } = true;

        // -----------------------------
        // SEAMOTH
        // -----------------------------

        [Toggle("Seamoth internal upgrades access")]
        public bool SeamothInternalUpgrades { get; set; } = true;

        [Toggle("Seamoth internal storage access")]
        public bool SeamothInternalStorage { get; set; } = true;

        // -----------------------------
        // PRAWN / EXOSUIT
        // -----------------------------

        [Toggle("Prawn internal upgrades access")]
        public bool ExosuitInternalUpgrades { get; set; } = true;

        [Toggle("Prawn internal storage access")]
        public bool ExosuitInternalStorage { get; set; } = true;

        // Si tu veux, on pourra rajouter ici d’autres options purement "live"
        // (par ex : activer / désactiver des features sans restart).
    }
}

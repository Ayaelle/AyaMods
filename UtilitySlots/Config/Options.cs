using Nautilus.Options.Attributes;
using Nautilus.Json;
using UnityEngine;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options Nautilus pour UtilitySlots.
    /// Toutes les propriétés publiques avec des attributs Nautilus
    /// apparaîtront dans le menu "UtilitySlots Settings".
    /// </summary>
    [Menu("UtilitySlots Settings")]
    public class Options : ConfigFile
    {
        /// <summary>
        /// Instance statique pour accéder facilement aux options depuis le code.
        /// </summary>
        public static Options Instance { get; private set; }

        public Options() : base("UtilitySlotsOptions")
        {
            Instance = this;
        }

        // =========================================================
        //  EXTRA SLOTS - Général
        // =========================================================

        /// <summary>
        /// Active ou désactive complètement la feature d'extension de slots.
        /// Pratique si tu veux garder le mod installé mais désactiver ses effets
        /// sans toucher aux sauvegardes.
        /// </summary>
        [Toggle("Enable extra slots feature")]
        public bool EnableExtraSlots { get; set; } = true;

        // =========================================================
        //  CHIPS (JOUEUR)
        // =========================================================

        /// <summary>
        /// Nombre de slots de chip maximum disponibles pour le joueur.
        /// Vanilla : 2. UtilitySlots permet d'aller au-delà.
        /// </summary>
        [Slider("Player chip slots", 2, 6)]
        public int ChipSlots { get; set; } = 4;

        // =========================================================
        //  SEAMOTH
        // =========================================================

        /// <summary>
        /// Nombre de slots modules pour le Seamoth.
        /// Vanilla : 4. UtilitySlots permet d'étendre au-delà.
        /// </summary>
        [Slider("Seamoth module slots", 4, 12)]
        public int SeamothSlots { get; set; } = 8;

        /// <summary>
        /// Active ou désactive les slots de "bras" pour le Seamoth.
        /// Ces bras seront gérés par la feature d'extension des slots.
        /// </summary>
        [Toggle("Enable Seamoth arm slots")]
        public bool SeamothArmSlots { get; set; } = true;

        /// <summary>
        /// Autorise l'accès interne aux upgrades / stockage du Seamoth
        /// via la touche configurée plus bas.
        /// </summary>
        [Toggle("Internal access (Seamoth)")]
        public bool SeamothInternalAccess { get; set; } = true;

        // =========================================================
        //  PRAWN (EXOSUIT)
        // =========================================================

        /// <summary>
        /// Nombre de slots modules pour l'Exosuit (Prawn).
        /// Vanilla : 4.
        /// </summary>
        [Slider("Prawn module slots", 4, 12)]
        public int ExosuitSlots { get; set; } = 8;

        /// <summary>
        /// Autorise l'accès interne au stockage de l'Exosuit
        /// via la touche d'accès interne.
        /// </summary>
        [Toggle("Internal access (Prawn)")]
        public bool ExosuitInternalAccess { get; set; } = true;

        // =========================================================
        //  CYCLOPS
        // =========================================================

        /// <summary>
        /// Nombre de slots modules pour le Cyclops.
        /// Vanilla : 6.
        /// </summary>
        [Slider("Cyclops module slots", 6, 14)]
        public int CyclopsSlots { get; set; } = 10;

        // =========================================================
        //  QUICKLOTS
        // =========================================================

        /// <summary>
        /// Active ou désactive complètement l'extension des quickslots (barre d'actions).
        /// </summary>
        [Toggle("Enable quickslot extension")]
        public bool EnableQuickslotExtension { get; set; } = true;

        /// <summary>
        /// Nombre de quickslots utilisables quand le joueur est à pied (nage libre).
        /// Vanilla : 4.
        /// </summary>
        [Slider("On-foot quickslots", 4, 12)]
        public int OnFootQuickslots { get; set; } = 8;

        /// <summary>
        /// Nombre de quickslots utilisables quand le joueur est dans un véhicule.
        /// Vanilla : 4.
        /// </summary>
        [Slider("In-vehicle quickslots", 4, 12)]
        public int VehicleQuickslots { get; set; } = 8;

        // =========================================================
        //  INTERNAL ACCESS GÉNÉRAL
        // =========================================================

        /// <summary>
        /// Active ou désactive la feature d'accès interne (depuis l'intérieur
        /// des véhicules, via une touche).
        /// </summary>
        [Toggle("Enable internal access feature")]
        public bool EnableInternalAccess { get; set; } = true;

        /// <summary>
        /// Touche utilisée pour déclencher l'accès interne (upgrades / stockage),
        /// quand le joueur est dans un véhicule compatible.
        /// </summary>
        [Keybind("Internal access key")]
        public KeyCode InternalAccessKey { get; set; } = KeyCode.U;
    }
}

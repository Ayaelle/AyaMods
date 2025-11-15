using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using System;
using UnityEngine;

namespace UtilitySlots.Config
{
    [Menu("Utility Slots")]
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

        // -----------------------------
        //   GENERAL SETTINGS
        // -----------------------------

        [Toggle("Enable Seamoth Arms")]
        public bool EnableSeamothArms = false;

        [Slider("Player Quickslots", 4, 12, DefaultValue = 4)]
        public int PlayerQuickSlots = 4;

        [Slider("Vehicle Quickslots", 4, 12, DefaultValue = 4)]
        public int VehicleQuickSlots = 4;

        // -----------------------------
        //   CHIP SLOTS
        // -----------------------------

        [Slider("Chip Slots", 1, 12, DefaultValue = 4)]
        public int ChipSlots = 4;

        // -----------------------------
        //   VEHICLE MODULE SLOTS
        // -----------------------------
        [Toggle("Enable Extra Slots")]
        public bool EnableExtraSlots { get; set; } = true;

        [Slider("Seamoth Module Slots", 2, 12, DefaultValue = 4)]
        public int SeamothModules = 4;

        [Slider("Exosuit Module Slots", 2, 12, DefaultValue = 4)]
        public int ExosuitModules = 4;

        [Slider("Cyclops Module Slots", 2, 12, DefaultValue = 6)]
        public int CyclopsModules = 6;

        // -----------------------------
        //   INTERNAL ACCESS
        // -----------------------------

        /// Active ou désactive la feature d'accès interne (depuis l'intérieur
        /// des véhicules, via des touches configurables).

        [Toggle("Enable internal access feature")]
        public bool EnableInternalAccess { get; set; } = true;

        /// Autorise l'accès aux UPGRADES depuis l'intérieur du véhicule.
        /// (par type de véhicule)

        [Toggle("Internal upgrades access (Seamoth)")]
        public bool SeamothInternalUpgrades { get; set; } = true;

        [Toggle("Internal upgrades access (Prawn)")]
        public bool ExosuitInternalUpgrades { get; set; } = true;

        /// Autorise l'accès au STOCKAGE interne depuis l'intérieur du véhicule.
        /// (par type de véhicule)

        [Toggle("Internal storage access (Seamoth)")]
        public bool SeamothInternalStorage { get; set; } = true;

        [Toggle("Internal storage access (Prawn)")]
        public bool ExosuitInternalStorage { get; set; } = true;

        // Tu pourras ajouter le Cyclops plus tard si on décide quoi faire pour lui.
        // [Toggle("Internal storage access (Cyclops)")]
        // public bool CyclopsInternalStorage { get; set; } = true;

        /// Touche utilisée pour ouvrir les upgrades depuis l'intérieur.

        [Keybind("Internal upgrades key")]
        public KeyCode InternalUpgradesKey { get; set; } = KeyCode.U;

        /// Touche utilisée pour ouvrir le stockage depuis l'intérieur.

        [Keybind("Internal storage key")]
        public KeyCode InternalStorageKey { get; set; } = KeyCode.I;

    }
}

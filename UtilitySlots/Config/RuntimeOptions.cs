//using BepInEx.Configuration;
using Nautilus.Json;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Options modifiables en jeu (onglet "Mods Options" + "Mods Input").
    /// Nautilus crée un fichier UtilitySlotsRuntime.json et gère
    /// automatiquement les sliders / toggles / keybinds.
    /// </summary>
    [Menu("Utility Slots (In-game options)")]
    public class RuntimeOptions : ConfigFile
    {
        /// <summary>
        /// Instance unique gérée par Nautilus.
        /// </summary>
        public static RuntimeOptions Instance { get; private set; }

        public RuntimeOptions() : base("UtilitySlotsRuntime")
        {
            Instance = this;
        }

        // -------- INTERNAL ACCESS (toggles) --------

        [Toggle("Enable internal access")]
        public bool EnableInternalAccess { get; set; } = true;

        [Toggle("Seamoth internal upgrades")]
        public bool SeamothInternalUpgrades { get; set; } = true;

        [Toggle("Seamoth internal storage")]
        public bool SeamothInternalStorage { get; set; } = true;

        [Toggle("Prawn internal upgrades")]
        public bool ExosuitInternalUpgrades { get; set; } = true;

        [Toggle("Prawn internal storage")]
        public bool ExosuitInternalStorage { get; set; } = true;

        // -------- KEYBINDS --------
        // Ces deux entrées apparaîtront dans le menu "Mods Input".

        [Keybind("Internal upgrades key")]
        public KeyCode InternalUpgradesKey { get; set; } = KeyCode.U;

        [Keybind("Internal storage key")]
        public KeyCode InternalStorageKey { get; set; } = KeyCode.I;
    }
}

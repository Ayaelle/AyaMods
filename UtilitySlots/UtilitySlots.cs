using AyaCoreMod.Core;
using AyaCoreMod.Features;
using BepInEx;
using HarmonyLib;
using Nautilus.Utility;
using System.Reflection;
using UnityEngine.SceneManagement;
using UtilitySlots.Config;
using CorePatchManager = AyaCoreMod.Core.PatchManager;

namespace UtilitySlots
{
    /// <summary>
    /// Point d'entrée BepInEx du mod UtilitySlots.
    /// S'appuie sur le core AyaCoreMod pour les patches, logs, gestion des features, etc.
    /// </summary>
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Subnautica.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string Guid = "com.ayaelle.ayamods.utilityslots";
        public const string Name = "UtilitySlots";
        public const string Version = "1.0.0";

        private Harmony _harmony;

        private void Awake()
        {
            // Connecte le système de log du core à celui de BepInEx
            Log.Bind(Logger);
            Log.Info($"{Name} Awake");

            // Enregistre les options Nautilus spécifiques au mod
            Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<Options>();
            Nautilus.Handlers.OptionsPanelHandler.RegisterModOptions<GlobalOptions>();
            UtilitySlots.Config.Keybinds.Register();

            if (FeatureFlags.SafeMode)
            {
                Log.Info("[UtilitySlots] SAFE MODE is enabled. Skipping patches and hooks.");
                return;
            }

            // Crée une instance Harmony dédiée à ce mod
            _harmony = new Harmony(Guid);
            CorePatchManager.ApplyAll(_harmony, Assembly.GetExecutingAssembly());

            // Hook sur le chargement de scène pour initialiser InputManager et les features
            SceneManager.sceneLoaded += OnSceneLoaded;

            Log.Info($"{Name} Awake END");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Initialise GameInput de manière sûre
            StartCoroutine(InputManager.DelayedInit());

            var gopt = GlobalOptions.Instance;

            // ExtraSlots : option globale
            FeatureRegistry.Enable<UtilitySlots.Features.ExtraSlotsFeature.ExtraSlotsFeature>();
            Log.Info("[UtilitySlots] ExtraSlotsFeature enabled from GlobalOptions.");
            // InternalAccess : activé TOUJOURS, la feature lit RuntimeInternalAccessConfig.EnableInternalAccess
            FeatureRegistry.Enable<UtilitySlots.Features.InternalAccessFeature.InternalAccessFeature>();
            Log.Info("[UtilitySlots] InternalAccessFeature enabled.");
            // Quickslots étendus : activés si l’option globale est cochée
            if (gopt.EnableQuickSlots)
            {
                FeatureRegistry.Enable<UtilitySlots.Features.QuickSlotsCore.QuickSlotsCoreFeature>();
                Log.Info("[UtilitySlots][Quickslots] QuickSlots Core enabled");
                FeatureRegistry.Enable<UtilitySlots.Features.QuickSlotsKeybinds.QuickSlotsKeybindsFeature>();
                Log.Info("[UtilitySlots][Quickslots] QuickSlots Keybinds enabled");
                FeatureRegistry.Enable<UtilitySlots.Features.QuickSlotsUI.QuickSlotsUIFeature>();
                Log.Info("[UtilitySlots][Quickslots] QuickSlots UI enabled");
                FeatureRegistry.Enable<UtilitySlots.Features.QuickSlotsVehicles.QuickSlotsVehicleFeature>();
                Log.Info("[UtilitySlots][Quickslots] QuickSlots Vehicle enabled");
                FeatureRegistry.Enable<UtilitySlots.Features.QuickSlotsCyclops.QuickSlotsCyclopsFeature>();
                Log.Info("[UtilitySlots][Quickslots] QuickSlots Cyclops enabled");
            }
            else
            {
                Log.Info("[UtilitySlots][Quickslots] Quickslots extension disabled in GlobalOptions; no Quickslots patches will be applied.");
            }

                SceneManager.sceneLoaded -= OnSceneLoaded;

            Log.Info("[UtilitySlots] Bootstrap complete.");
        }

        private void OnDestroy()
        {
            // Nettoyage propre
            FeatureRegistry.DisableAll();
            _harmony?.UnpatchSelf();
        }
    }
}

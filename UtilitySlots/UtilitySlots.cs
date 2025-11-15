using AyaCoreMod.Core;
using AyaCoreMod.Features;
using BepInEx;
using HarmonyLib;
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
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Initialise GameInput de manière sûre
            StartCoroutine(InputManager.DelayedInit());

            // Récupère une fois les options (si Nautilus les a déjà créées)
            var opt = Options.Instance;

            // Active notre première feature (slots étendus)
            // Si tu veux la rendre optionnelle, tu peux utiliser opt.EnableExtraSlots
            if (opt != null && opt.EnableExtraSlots)
                FeatureRegistry.Enable<Features.ExtraSlotsFeature.ExtraSlotsFeature>();

            if (opt != null && opt.EnableInternalAccess)
                FeatureRegistry.Enable<Features.InternalAccessFeature.InternalAccessFeature>();

            // ⚠️ QuickslotExtensionFeature n'existe pas encore → on désactive pour l'instant.
            //
            // if (opt != null && opt.EnableQuickslotExtension)
            //     FeatureRegistry.Enable<Features.QuickslotExtensionFeature.QuickslotExtensionFeature>();
            //
            // TODO : quand tu créeras la feature de quickslots étendus,
            //        tu pourras décommenter ces lignes et ajouter la classe correspondante.

            // On n'a besoin de bootstrapper qu'une seule fois
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

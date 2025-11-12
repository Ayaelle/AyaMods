using AyaCoreMod.Core;
using AyaCoreMod.Feature;
using AyaCoreMod.UtilitySlots.Config;
using BepInEx;
using HarmonyLib;
using Nautilus.Handlers;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UtilitySlots.Config;

namespace AyaCoreMod.UtilitySlots
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.snmodding.nautilus", BepInDependency.DependencyFlags.HardDependency)]
    [BepInProcess("Subnautica.exe")]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = "com.ayaelle.ayamods.utilityslots";
        public const string MODNAME = "AyaMods.UtilitySlots";
        public const string VERSION = "1.0.0";

        internal static Harmony Harmony;

        void Awake()
        {
            Log.Bind(Logger);
            Log.Info($"{MODNAME} Awake");

            OptionsPanelHandler.RegisterModOptions<Options>();
            if (FeatureFlags.SafeMode) { Log.Info("[UtilitySlots] SAFE MODE ON"); return; }

            Harmony = new Harmony(GUID);
            PatchManager.ApplyAll(Harmony, Assembly.GetExecutingAssembly());

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene s, LoadSceneMode m)
        {
            StartCoroutine(InputManager.DelayedInit());

            // Activer nos features ici (via le Core)
            FeatureRegistry.Enable<Features.ExtraSlotsFeature.ExtraSlotsFeature>();
            if (Options.Instance.EnableInternalAccess)
                FeatureRegistry.Enable<Features.InternalAccessFeature.InternalAccessFeature>();
            if (Options.Instance.EnableQuickslotExtension)
                FeatureRegistry.Enable<Features.QuickslotExtensionFeature.QuickslotExtensionFeature>();

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnDestroy()
        {
            FeatureRegistry.DisableAll();
            Harmony?.UnpatchSelf();
        }
    }
}


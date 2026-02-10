using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    internal static class ExtraSlotsVehiclesRuntime
    {
        private const int VanillaSeamoth = 4;
        private const int VanillaExosuit = 4;  // (les bras sont séparés : ExosuitArmLeft/Right)
        private const int VanillaCyclops = 6;

        public static bool IsEnabled()
            => UtilitySlots.Config.RuntimeConfig.EnableExtraSlots;

        public static int DesiredSeamothModules()
            => UtilitySlots.Config.RuntimeConfig.SeamothModuleSlots;

        public static int DesiredExosuitModules()
            => UtilitySlots.Config.RuntimeConfig.ExosuitModuleSlots;

        public static int DesiredCyclopsModules()
            => UtilitySlots.Config.RuntimeConfig.CyclopsModuleSlots;

        public static void ExpandSeaMoth(Equipment modules)
            => TryExpand(modules, "Seamoth", VanillaSeamoth, DesiredSeamothModules(), prefix: "SeamothModule");

        public static void ExpandExosuit(Equipment modules)
            => TryExpand(modules, "Exosuit", VanillaExosuit, DesiredExosuitModules(), prefix: "ExosuitModule");

        public static void ExpandCyclops(Equipment modules)
            => TryExpand(modules, "Cyclops", VanillaCyclops, DesiredCyclopsModules(), prefix: "Module");

        private static void TryExpand(Equipment equipment, string tag, int vanilla, int desired, string prefix)
        {
            if (!IsEnabled()) return;
            if (equipment == null) return;

            if (desired < vanilla) desired = vanilla;
            if (desired == vanilla) return;

            // On ajoute seulement les slots manquants (vanilla+1..desired)
            // Equipment.AddSlot(string) existe bien dans tes assemblies.
            for (int i = vanilla + 1; i <= desired; i++)
            {
                string slotId = $"{prefix}{i}";
                equipment.AddSlot(slotId);
            }

            Log.Info($"[UtilitySlots][ExtraSlotsVehicles][{tag}] slots étendus à {desired} ({prefix}1..{prefix}{desired}).");
        }
    }
}

using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    internal static class ExtraSlotsVehiclesPatches
    {
        [HarmonyPatch(typeof(SeaMoth), "Start")]
        private static class SeaMoth_Start
        {
            static void Postfix(SeaMoth __instance)
            {
                if (__instance == null) return;
                ExtraSlotsVehiclesRuntime.ExpandSeaMoth(__instance.modules);
            }
        }

        [HarmonyPatch(typeof(Exosuit), "Start")]
        private static class Exosuit_Start
        {
            static void Postfix(Exosuit __instance)
            {
                if (__instance == null) return;
                ExtraSlotsVehiclesRuntime.ExpandExosuit(__instance.modules);
            }
        }
    }
}

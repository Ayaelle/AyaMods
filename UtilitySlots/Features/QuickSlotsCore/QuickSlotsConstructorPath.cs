using System;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Étend QuickSlots lors de sa création pour supporter jusqu'à HardMaxSlots.
    /// </summary>
    [HarmonyPatch(typeof(QuickSlots))]
    public static class QuickSlotsConstructorPatch
    {
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[]
        {
            typeof(GameObject),
            typeof(Transform),
            typeof(Transform),
            typeof(Inventory),
            typeof(Transform),
            typeof(int)
        })]
        [HarmonyPrefix]
        public static void QuickSlots_Ctor_Prefix(ref int slotCount)
        {
            QuickSlotsCoreFeature.EnsureRunner();

            int target = QuickSlotsRuntime.GetPhysicalSlots();

            if (target > slotCount)
            {
                Log.Info($"[UtilitySlots][Quickslots] Extending QuickSlots ctor: {slotCount} -> {target}");
                slotCount = target;
            }
        }
    }
}
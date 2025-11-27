using AyaCoreMod.Core;
using HarmonyLib;
using System;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Patches logiques QuickSlots:
    /// - GetSlotBinding : expose N slots à l'UI.
    /// - GetSlotCount   : nombre de slots "logiques".
    /// - Select / Bind  : bloquent hors plage.
    /// - SlotNext/Previous : respectent la limite.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsLogicPatches
    {
        [HarmonyPatch(typeof(QuickSlots), "GetSlotBinding", new System.Type[] { })]
        [HarmonyPostfix]
        public static void GetSlotBinding_Postfix(QuickSlots __instance, ref TechType[] __result)
        {
            if (__result == null)
                return;

            int physical = __result.Length;
            if (physical <= 0)
                return;

            int visible;

            if (!RuntimeConfig.EnableQuickSlots)
            {
                visible = Math.Min(physical, 5);
            }
            else
            {
                visible = Mathf.Clamp(RuntimeConfig.OnFootQuickslots, 1, QuickSlotsRuntime.HardMaxSlots);
                visible = Math.Min(visible, physical);
            }

            if (visible == physical)
                return;

            var trimmed = new TechType[visible];
            Array.Copy(__result, trimmed, visible);
            __result = trimmed;
        }

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.GetSlotCount))]
        [HarmonyPostfix]
        public static void GetSlotCount_Postfix(QuickSlots __instance, ref int __result)
        {
            int physical = __result; // valeur réelle du QuickSlots

            if (!RuntimeConfig.EnableQuickSlots)
            {
                // Mode "vanilla-like" : max 5 slots visibles
                __result = Math.Min(physical, 5);
                return;
            }

            // Mode mod : OnFoot uniquement pour l'instant
            int configured = RuntimeConfig.OnFootQuickslots;
            configured = Mathf.Clamp(configured, 1, QuickSlotsRuntime.HardMaxSlots);

            __result = Math.Min(physical, configured);
        }

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Select))]
        [HarmonyPrefix]
        public static bool Select_Prefix(QuickSlots __instance, int slotID)
        {
            var context = QuickSlotsRuntime.GetCurrentContext();
            int requested = QuickSlotsRuntime.GetConfiguredSlots(context);
            int maxSlots = Math.Max(0, Math.Min(__instance.GetSlotCount(), requested));

            if (slotID < 0 || slotID >= maxSlots)
            {
                Log.Info(
                    $"[UtilitySlots][Quickslots] Select bloqué : slot={slotID}, contexte={context}, maxSlots={maxSlots}"
                );
                return false;
            }

            return true;
        }

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.Bind))]
        [HarmonyPrefix]
        public static bool Bind_Prefix(QuickSlots __instance, int slotID, InventoryItem item)
        {
            var context = QuickSlotsRuntime.GetCurrentContext();
            int requested = QuickSlotsRuntime.GetConfiguredSlots(context);
            int maxSlots = Math.Max(0, Math.Min(__instance.GetSlotCount(), requested));

            if (slotID < 0 || slotID >= maxSlots)
            {
                Log.Info(
                    $"[UtilitySlots][Quickslots] Bind bloqué : slot={slotID}, contexte={context}, maxSlots={maxSlots}"
                );
                return false;
            }

            return true;
        }

        // Optionnel : s'assurer que SlotNext / SlotPrevious respectent la limite
        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.SlotNext))]
        [HarmonyPrefix]
        public static bool SlotNext_Prefix(QuickSlots __instance)
        {
            // On laisse la logique vanilla, mais le GetSlotCount patché
            // et Select/Bind suffisent en général. Ici, juste log si besoin.
            return true;
        }

        [HarmonyPatch(typeof(QuickSlots), nameof(QuickSlots.SlotPrevious))]
        [HarmonyPrefix]
        public static bool SlotPrevious_Prefix(QuickSlots __instance)
        {
            return true;
        }
    }
}
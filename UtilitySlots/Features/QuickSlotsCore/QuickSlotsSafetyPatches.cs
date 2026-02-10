using System;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsCore
{
    [HarmonyPatch(typeof(QuickSlots))]
    public static class QuickSlotsSafetyPatches
    {
        private static readonly FieldInfo BindingField =
            AccessTools.Field(typeof(QuickSlots), "binding");

        private static readonly FieldInfo SlotCountField =
            AccessTools.Field(typeof(QuickSlots), "slotCount");

        private static readonly FieldInfo ActiveSlotField =
            AccessTools.Field(typeof(QuickSlots), "activeSlot");

        private static readonly FieldInfo DesiredSlotField =
            AccessTools.Field(typeof(QuickSlots), "desiredSlot");

        // ⚠️ Autres tableaux internes touchés par SelectInternal
        private static readonly FieldInfo IconsField =
            AccessTools.Field(typeof(QuickSlots), "icons");

        private static readonly FieldInfo SlotsField =
            AccessTools.Field(typeof(QuickSlots), "slots");

        private static readonly FieldInfo SlotButtonsField =
            AccessTools.Field(typeof(QuickSlots), "slotButtons");

        /// <summary>
        /// Slot count logique (GetSlotCount borné par binding)
        /// </summary>
        private static int GetLogicalSafeCount(QuickSlots qs, InventoryItem[] binding)
        {
            int physical = binding?.Length ?? 0;

            int logical;
            try
            {
                logical = qs.GetSlotCount();
            }
            catch
            {
                logical = physical;
            }

            if (logical < 0) logical = 0;
            if (logical > physical) logical = physical;

            return logical;
        }

        /// <summary>
        /// Slot count HARD = min de tous les tableaux internes existants
        /// </summary>
        private static int GetHardSafeCount(QuickSlots qs, InventoryItem[] binding)
        {
            int min = binding?.Length ?? 0;

            void ClampWith(FieldInfo f)
            {
                if (f == null) return;
                if (f.GetValue(qs) is Array arr && arr.Length > 0)
                    min = (min == 0) ? arr.Length : Math.Min(min, arr.Length);
            }

            ClampWith(IconsField);
            ClampWith(SlotsField);
            ClampWith(SlotButtonsField);

            return min;
        }

        // ===============================
        // SelectInternal
        // ===============================
        [HarmonyPatch("SelectInternal")]
        [HarmonyPrefix]
        private static bool SelectInternal_Prefix(QuickSlots __instance, ref int slotID)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];
            if (binding == null)
                return true;

            int logical = GetLogicalSafeCount(__instance, binding);
            int hard = GetHardSafeCount(__instance, binding);
            int safeCount = Math.Min(logical, hard);

            // Corrige slotCount si incohérent
            if (SlotCountField != null)
            {
                object raw = SlotCountField.GetValue(__instance);
                if (raw is int rawCount && (rawCount < 0 || rawCount > safeCount))
                {
                    SlotCountField.SetValue(__instance, safeCount);
                }
            }

            if (safeCount <= 0)
            {
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false;
            }

            if (slotID < 0 || slotID >= safeCount)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] SelectInternal blocked slotID={slotID} safeCount={safeCount}");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false;
            }

            return true;
        }

        // ===============================
        // DeselectInternal
        // ===============================
        [HarmonyPatch("DeselectInternal")]
        [HarmonyPrefix]
        private static bool DeselectInternal_Prefix(QuickSlots __instance)
        {
            var binding = BindingField?.GetValue(__instance) as InventoryItem[];
            if (binding == null)
                return true;

            int logical = GetLogicalSafeCount(__instance, binding);
            int hard = GetHardSafeCount(__instance, binding);
            int safeCount = Math.Min(logical, hard);

            int activeSlot = -1;
            if (ActiveSlotField != null)
            {
                object raw = ActiveSlotField.GetValue(__instance);
                if (raw is int i)
                    activeSlot = i;
            }

            if (activeSlot < 0)
                return true;

            if (activeSlot >= safeCount)
            {
                Log.Warn($"[UtilitySlots][Quickslots][Safety] DeselectInternal blocked activeSlot={activeSlot} safeCount={safeCount}");
                ActiveSlotField?.SetValue(__instance, -1);
                DesiredSlotField?.SetValue(__instance, -1);
                return false; // ⚠️ IMPORTANT : on skip le vanilla
            }

            return true;
        }
    }
}

using System;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsCore
{
    /// <summary>
    /// Étend QuickSlots.slotNames pour supporter plus de 6 slots
    /// (jusqu'à HardMaxSlots, 12), afin d'éviter les IndexOutOfRange.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsSlotNamesPatch
    {
        private static readonly FieldInfo SlotNamesField =
            AccessTools.Field(typeof(QuickSlots), "slotNames");

        static QuickSlotsSlotNamesPatch()
        {
            try
            {
                if (SlotNamesField == null)
                    return;

                var current = SlotNamesField.GetValue(null) as string[];
                if (current == null || current.Length >= QuickSlotsRuntime.HardMaxSlots)
                    return;

                int oldLen = current.Length;
                int newLen = QuickSlotsRuntime.HardMaxSlots;

                var extended = new string[newLen];
                Array.Copy(current, extended, oldLen);

                for (int i = oldLen; i < newLen; i++)
                {
                    extended[i] = $"QuickSlot{i}";
                }

                SlotNamesField.SetValue(null, extended);

                Log.Info($"[UtilitySlots][Quickslots] slotNames étendu de {oldLen} à {newLen} entrées.");
            }
            catch (Exception e)
            {
                Log.Error($"[UtilitySlots][Quickslots] Impossible d'étendre slotNames : {e}");
            }
        }
    }
}
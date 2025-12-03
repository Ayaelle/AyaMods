using HarmonyLib;
using AyaCoreMod.Core;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlotsCore
{
    [HarmonyPatch(typeof(Inventory), "UnlockDefaultEquipmentSlots")]
    internal static class ExtraSlotsUnlockPatch
    {
        static void Postfix(Inventory __instance)
        {
            try
            {
                int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
                if (desired <= ExtraSlotsRuntime.VanillaChipSlots)
                    return;

                var eq = __instance.equipment;
                if (eq == null)
                    return;

                for (int i = ExtraSlotsRuntime.VanillaChipSlots + 1; i <= desired; i++)
                {
                    string slot = $"Chip{i}";
                    Log.Info($"[UtilitySlots][ExtraSlotsCore][UnlockPatch] Adding chip slot '{slot}' via Equipment.AddSlots().");
                    eq.AddSlots(new[] { slot });
                }
            }
            catch (System.Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsCore][UnlockPatch] Exception: " + e);
            }
        }
    }
}

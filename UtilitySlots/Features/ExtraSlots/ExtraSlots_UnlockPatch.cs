using HarmonyLib;
using AyaCoreMod.Core;

namespace UtilitySlots.Features.ExtraSlots
{
    [HarmonyPatch(typeof(Inventory), "UnlockDefaultEquipmentSlots")]
    internal static class ExtraSlotsUnlockPatch
    {
        static void Postfix(Inventory __instance)
        {
            try
            {
                int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
                if (desired <= 2) return;

                var eq = __instance.equipment;
                if (eq == null) return;

                for (int i = 3; i <= desired; i++)
                {
                    string slot = $"Chip{i}";
                    Log.Info($"[UtilitySlots][ExtraSlots][UnlockPatch] Adding chip slot '{slot}' via Equipment.AddSlots().");

                    eq.AddSlots(new[] { slot });
                }
            }
            catch (System.Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][UnlockPatch] Exception: " + e);
            }
        }
    }
}

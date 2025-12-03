using System;
using AyaCoreMod.Core;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helper pour étendre réellement l’Equipment du joueur
    /// (ajout des slots Chip3..Chip6 via Equipment.AddSlot).
    /// </summary>
    public static class ExtraSlotsPlayerRuntime
    {
        public static void ExpandChipSlots(Equipment equipment)
        {
            if (equipment == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][Player] ExpandChipSlots called with null equipment.");
                return;
            }

            int hard = ExtraSlotsRuntime.GetHardChipSlots();
            if (hard <= ExtraSlotsRuntime.VanillaChipSlots)
                return;

            for (int i = ExtraSlotsRuntime.VanillaChipSlots + 1; i <= hard; i++)
            {
                string slotId = $"Chip{i}";
                try
                {
                    Log.Info($"[UtilitySlots][ExtraSlots][Player] Ensuring Equipment slot '{slotId}' exists via AddSlot().");
                    equipment.AddSlot(slotId);
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlots][Player] Exception while AddSlot('" + slotId + "'): " + e);
                }
            }

            int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
            Log.Info($"[UtilitySlots][ExtraSlots][Player] Chip slots expanded (hard) up to: {hard}. Desired/active: {desired}.");
        }
    }
}

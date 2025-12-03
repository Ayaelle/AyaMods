using System;
using AyaCoreMod.Core;
using UtilitySlots.Features.ExtraSlotsCore;

namespace UtilitySlots.Features.ExtraSlotsPlayer
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
                Log.Warn("[UtilitySlots][ExtraSlotsCore][Player] ExpandChipSlots called with null equipment.");
                return;
            }

            int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
            if (desired <= ExtraSlotsRuntime.VanillaChipSlots)
                return;

            for (int i = ExtraSlotsRuntime.VanillaChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";
                try
                {
                    Log.Info($"[UtilitySlots][ExtraSlotsCore][Player] Ensuring Equipment slot '{slotId}' exists via AddSlot().");
                    equipment.AddSlot(slotId);
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlotsCore][Player] Exception while AddSlot('" + slotId + "'): " + e);
                }
            }

            Log.Info($"[UtilitySlots][ExtraSlotsCore][Player] Chip slots expanded up to: {desired}.");
        }
    }
}

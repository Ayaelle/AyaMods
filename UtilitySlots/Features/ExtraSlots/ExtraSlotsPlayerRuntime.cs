using System;
using AyaCoreMod.Core;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helper qui étend réellement l'Equipment du joueur
    /// (ajout de Chip3 / Chip4 via Equipment.AddSlot).
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

            int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
            if (desired <= ExtraSlotsRuntime.VanillaChipSlots)
                return;

            for (int i = ExtraSlotsRuntime.VanillaChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";
                try
                {
                    // Equipment.AddSlot se base sur slotMapping (qu'on a patché plus haut).
                    Log.Info($"[UtilitySlots][ExtraSlots][Player] Ensuring Equipment slot '{slotId}' exists via AddSlot().");
                    equipment.AddSlot(slotId);
                }
                catch (Exception e)
                {
                    Log.Error("[UtilitySlots][ExtraSlots][Player] Exception while AddSlot('" + slotId + "'): " + e);
                }
            }

            Log.Info($"[UtilitySlots][ExtraSlots][Player] Chip slots expanded up to: {desired}.");
        }
    }
}

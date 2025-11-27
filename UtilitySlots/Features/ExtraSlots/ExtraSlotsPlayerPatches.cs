using System.Collections.Generic;
using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Logique runtime liée au Equipment du joueur :
    /// on ajoute Chip3..ChipN sur l'instance d'Equipment du joueur.
    /// </summary>
    [HarmonyPatch]
    internal static class ExtraSlotsPlayerPatches
    {
        /// <summary>
        /// Appelé depuis ExtraSlotsFeature.Runner quand Inventory.main.equipment est prêt.
        /// </summary>
        public static void ExpandChipSlots(Equipment equipment)
        {
            if (equipment == null)
                return;

            if (!ExtraSlotsRuntime.IsEnabled())
                return;

            int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
            if (desired <= ExtraSlotsRuntime.MinChipSlots)
                return;

            Log.Info("[UtilitySlots][ExtraSlots][Player] Expanding player chip slots…");

            // Récupérer les slots Chip déjà définis sur cet Equipment
            var existing = new List<string>();
            equipment.GetSlots(EquipmentType.Chip, existing);

            Log.Info($"[UtilitySlots][ExtraSlots][Player] Existing chip slots ({existing.Count}): {string.Join(", ", existing)}; desired: {desired}");

            // Ajouter les slots Chip3..ChipN si besoin
            for (int i = ExtraSlotsRuntime.MinChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";
                if (!existing.Contains(slotId))
                {
                    Log.Info($"[UtilitySlots][ExtraSlots][Player] Ensuring Equipment slot '{slotId}' exists via AddSlot().");
                    equipment.AddSlot(slotId);
                }
            }

            existing.Clear();
            equipment.GetSlots(EquipmentType.Chip, existing);
            Log.Info($"[UtilitySlots][ExtraSlots][Player] Chip slots expanded up to: {existing.Count}.");
        }
    }
}

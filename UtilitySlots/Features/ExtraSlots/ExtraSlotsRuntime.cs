using AyaCoreMod.Core;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Runtime helper pour tout ce qui concerne les slots supplémentaires.
    /// Pour l'instant : uniquement les slots de puces du joueur.
    /// </summary>
    public static class ExtraSlotsRuntime
    {
        public const int VanillaChipSlots = 2;
        public const int MaxChipSlots = 6;

        private static bool _chipsInitialized;

        /// <summary>
        /// Appelé depuis le runner de ExtraSlotsFeature.
        /// Étend les slots de puces du joueur (gameplay, pas UI).
        /// </summary>
        public static void EnsurePlayerChipSlots()
        {
            if (_chipsInitialized)
                return;

            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            int desired = Mathf.Clamp(gopt.ChipSlots, VanillaChipSlots, MaxChipSlots);

            // Rien à faire si on reste sur le vanilla.
            if (desired <= VanillaChipSlots)
            {
                Log.Info("[UtilitySlots][ExtraSlots][Player] ChipSlots = vanilla, no expansion.");
                _chipsInitialized = true;
                return;
            }

            // On s'assure que les nouveaux slots sont connus du système d'équipement.
            try
            {
                ExpandPlayerChipEquipment(desired);
                _chipsInitialized = true;
            }
            catch (System.Exception ex)
            {
                Log.Error($"[UtilitySlots][ExtraSlots][Player] EnsurePlayerChipSlots failed: {ex}");
            }
        }

        /// <summary>
        /// Étend les slots de puces côté Equipment/Inventory :
        /// - ajoute Chip3..ChipN dans Equipment.slotMapping (type Chip)
        /// - appelle Inventory.main.equipment.AddSlot("ChipX") pour chaque nouveau slot.
        /// </summary>
        private static void ExpandPlayerChipEquipment(int desired)
        {
            Log.Info($"[UtilitySlots][ExtraSlots][Player] Expanding chip slots up to {desired}.");

            // 1) Étendre le slotMapping statique pour Chip3..ChipN
            for (int i = VanillaChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";

                if (!Equipment.slotMapping.ContainsKey(slotId))
                {
                    Equipment.slotMapping.Add(slotId, EquipmentType.Chip);
                    Log.Info($"[UtilitySlots][ExtraSlots][Player] slotMapping['{slotId}'] = EquipmentType.Chip");
                }
            }

            // 2) Ajouter réellement les slots dans l'équipement du joueur
            Inventory inv = Inventory.main;
            if (inv == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][Player] Inventory.main is null; will retry later.");
                return;
            }

            Equipment eq = inv.equipment;
            if (eq == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][Player] Inventory.main.equipment is null; will retry later.");
                return;
            }

            for (int i = VanillaChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";
                // AddSlot est idempotent : si le slot existe déjà, il ne fera rien.
                bool added = eq.AddSlot(slotId);
                Log.Info($"[UtilitySlots][ExtraSlots][Player] AddSlot('{slotId}') -> {added}");
            }
        }
    }
}

using System.Collections.Generic;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patch UI sur uGUI_Equipment.Init pour gérer l'affichage des slots de puces supplémentaires.
    /// - On clone visuellement Chip1 / Chip2 pour créer Chip3..ChipN (N <= 6).
    /// - On place les rangées supplémentaires AU-DESSUS des deux slots vanilla.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), "Init")]
    public static class ExtraSlotsPlayerUIPatches
    {
        [HarmonyPostfix]
        public static void Init_Postfix(uGUI_Equipment __instance, Equipment equipment)
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            int desired = Mathf.Clamp(gopt.ChipSlots, ExtraSlotsRuntime.VanillaChipSlots, ExtraSlotsRuntime.MaxChipSlots);
            if (desired <= ExtraSlotsRuntime.VanillaChipSlots)
                return;

            if (equipment == null)
                return;

            // Récupérer les slots de type Chip côté gameplay
            var chipSlotIDs = new List<string>();
            equipment.GetSlots(EquipmentType.Chip, chipSlotIDs);

            if (chipSlotIDs.Count <= ExtraSlotsRuntime.VanillaChipSlots)
                return; // rien de plus que Chip1/Chip2 en gameplay -> pas d'UI à créer

            // On récupère la map interne allSlots de uGUI_Equipment via réflexion.
            var allSlotsField = AccessTools.Field(typeof(uGUI_Equipment), "allSlots");
            var dict = allSlotsField?.GetValue(__instance) as Dictionary<string, uGUI_EquipmentSlot>;
            if (dict == null)
                return;

            if (!dict.TryGetValue("Chip1", out var chip1Slot) ||
                !dict.TryGetValue("Chip2", out var chip2Slot))
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Chip1 / Chip2 UI slots not found.");
                return;
            }

            RectTransform chip1Rect = chip1Slot.iconRect;
            RectTransform chip2Rect = chip2Slot.iconRect;
            if (chip1Rect == null || chip2Rect == null)
                return;

            // Position de base : moyenne des deux slots vanilla.
            float baseY = (chip1Rect.anchoredPosition.y + chip2Rect.anchoredPosition.y) * 0.5f;
            float leftX = chip1Rect.anchoredPosition.x;
            float rightX = chip2Rect.anchoredPosition.x;

            float slotHeight = chip1Rect.rect.height;
            // Espacement vertical : ~1.1 * la hauteur du slot (on peut ajuster à ton goût).
            float verticalSpacing = slotHeight * 1.1f;

            Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] baseY={baseY}, leftX={leftX}, rightX={rightX}, slotHeight={slotHeight}, vSpacing={verticalSpacing}");

            // Créer/positionner Chip3..ChipN en rangées de 2 (gauche/droite) AU-DESSUS
            // des slots vanilla (Chip1/Chip2).
            for (int i = ExtraSlotsRuntime.VanillaChipSlots + 1; i <= desired; i++)
            {
                string slotId = $"Chip{i}";
                if (!dict.TryGetValue(slotId, out var uiSlot))
                {
                    // Cloner à partir de Chip1 ou Chip2 pour garder le même style.
                    uGUI_EquipmentSlot source = (i % 2 == 1) ? chip1Slot : chip2Slot;
                    GameObject clone = Object.Instantiate(source.gameObject, source.transform.parent);
                    clone.name = $"Chip{i}_Slot";

                    uiSlot = clone.GetComponent<uGUI_EquipmentSlot>();
                    if (uiSlot == null)
                    {
                        Log.Warn($"[UtilitySlots][ExtraSlots][PlayerUI] Clone for '{slotId}' has no uGUI_EquipmentSlot, aborting for this slot.");
                        Object.Destroy(clone);
                        continue;
                    }

                    uiSlot.slot = slotId;
                    uiSlot.manager = __instance;

                    dict[slotId] = uiSlot;

                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Created uGUI slot for '{slotId}'.");
                }

                RectTransform r = uiSlot.iconRect ?? uiSlot.GetComponent<RectTransform>();
                if (r == null)
                    continue;

                // index logique des extra slots : Chip3 -> 0, Chip4 -> 1, Chip5 -> 2, ...
                int extraIndex = i - (ExtraSlotsRuntime.VanillaChipSlots + 1);
                int row = extraIndex / 2; // 0 pour 3/4, 1 pour 5/6, etc.
                bool isLeft = (i % 2 == 1);

                float targetX = isLeft ? leftX : rightX;
                // AU-DESSUS des slots vanilla : baseY + (row+1) * verticalSpacing
                float targetY = baseY + (row + 1) * verticalSpacing;

                var pos = r.anchoredPosition;
                pos.x = targetX;
                pos.y = targetY;
                r.anchoredPosition = pos;

                uiSlot.SetActive(true);

                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Positioned '{slotId}' at ({targetX}, {targetY}).");
            }
        }
    }
}

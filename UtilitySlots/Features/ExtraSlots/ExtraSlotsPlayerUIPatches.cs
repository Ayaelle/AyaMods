using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patches UI du PDA (uGUI_Equipment) pour afficher Chip3 / Chip4.
    /// On clone les slots Chip1 / Chip2, on les renomme et on les repositionne.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), "Init")]
    internal static class ExtraSlotsPlayerUIPatches
    {
        private static readonly FieldInfo AllSlotsField =
            AccessTools.Field(typeof(uGUI_Equipment), "allSlots");

        static void Postfix(uGUI_Equipment __instance, Equipment equipment)
        {
            try
            {
                if (!ExtraSlotsRuntime.IsEnabled())
                    return;

                if (equipment == null)
                    return;

                // Ne cible que le joueur (PDA du joueur).
                if (equipment != Inventory.main?.equipment)
                    return;

                int desired = ExtraSlotsRuntime.GetDesiredChipSlots();
                if (desired <= ExtraSlotsRuntime.VanillaChipSlots)
                    return;

                var allSlotsObj = AllSlotsField?.GetValue(__instance);
                var allSlots = allSlotsObj as Dictionary<string, uGUI_EquipmentSlot>;
                if (allSlots == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] allSlots dictionary is null.");
                    return;
                }

                if (!allSlots.TryGetValue("Chip1", out var chip1) ||
                    !allSlots.TryGetValue("Chip2", out var chip2) ||
                    chip1 == null || chip2 == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Could not find Chip1/Chip2 UI slots.");
                    return;
                }

                var chip1Rect = chip1.GetComponent<RectTransform>();
                var chip2Rect = chip2.GetComponent<RectTransform>();
                if (chip1Rect == null || chip2Rect == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Chip1/Chip2 RectTransform missing.");
                    return;
                }

                // Calcul de layout à partir des slots vanilla
                float slotHeight = Mathf.Abs(chip1Rect.rect.height);
                float rowOffset = slotHeight * 0.9f; // rangée du dessus
                float colInset = Mathf.Abs(chip2Rect.anchoredPosition.x - chip1Rect.anchoredPosition.x) * 0.25f;

                Vector2 p1 = chip1Rect.anchoredPosition;
                Vector2 p2 = chip2Rect.anchoredPosition;

                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] slotHeight={slotHeight:F1}, rowOffset={rowOffset:F1}, colInset={colInset:F1}, chip1Pos={p1}, chip2Pos={p2}");

                // Chip3 (gauche, rangée du haut, vers l'extérieur)
                if (desired >= 3 && !allSlots.ContainsKey("Chip3"))
                {
                    var ui = CreateChipSlot(__instance, "Chip3", chip1, chip1Rect,
                        horizontalOffset: -colInset,
                        verticalOffset: rowOffset);
                    allSlots["Chip3"] = ui;
                }

                // Chip4 (droite, rangée du haut, vers l'extérieur)
                if (desired >= 4 && !allSlots.ContainsKey("Chip4"))
                {
                    var ui = CreateChipSlot(__instance, "Chip4", chip2, chip2Rect,
                        horizontalOffset: +colInset,
                        verticalOffset: rowOffset);
                    allSlots["Chip4"] = ui;
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][PlayerUI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }

        private static uGUI_EquipmentSlot CreateChipSlot(
            uGUI_Equipment manager,
            string slotId,
            uGUI_EquipmentSlot template,
            RectTransform templateRect,
            float horizontalOffset,
            float verticalOffset)
        {
            var parent = template.transform.parent;
            var cloneGO = UnityEngine.Object.Instantiate(template.gameObject, parent);
            cloneGO.name = $"{template.gameObject.name}_{slotId}";

            var slot = cloneGO.GetComponent<uGUI_EquipmentSlot>();
            var rect = cloneGO.GetComponent<RectTransform>();

            slot.slot = slotId;
            slot.manager = manager;
            slot.SetActive(true);

            Vector2 basePos = templateRect.anchoredPosition;
            rect.anchoredPosition = new Vector2(
                basePos.x + horizontalOffset,
                basePos.y + verticalOffset
            );

            // On s'assure que le slot commence "vide"
            slot.ClearIcon();

            Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Created uGUI slot for '{slotId}' at {rect.anchoredPosition}.");
            return slot;
        }
    }
}

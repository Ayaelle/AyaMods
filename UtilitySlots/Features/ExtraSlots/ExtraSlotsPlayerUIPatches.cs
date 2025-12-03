using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Patches UI du PDA (uGUI_Equipment) pour afficher Chip3..Chip6.
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

                if (equipment != Inventory.main?.equipment)
                    return;

                int desired = ExtraSlotsRuntime.GetDesiredChipSlots();

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

                float slotHeight = Mathf.Abs(chip1Rect.rect.height);
                float rowOffset = slotHeight * 0.9f;
                float colInset = Mathf.Abs(chip2Rect.anchoredPosition.x - chip1Rect.anchoredPosition.x) * 0.25f;

                Vector2 p1 = chip1Rect.anchoredPosition;
                Vector2 p2 = chip2Rect.anchoredPosition;

                Log.Info(
                    $"[UtilitySlots][ExtraSlots][PlayerUI] " +
                    $"slotHeight={slotHeight:F1}, rowOffset={rowOffset:F1}, " +
                    $"colInset={colInset:F1}, chip1Pos={p1}, chip2Pos={p2}"
                );

                // Création/réutilisation de Chip3..Chip6 puis activation selon desired
                for (int i = 3; i <= desired; i++)
                {
                    string slotId = $"Chip{i}";

                    bool isLeft = (i % 2) == 1;     // 3,5 = gauche ; 4,6 = droite
                    bool isUpperRow = (i <= 4);    // 3,4 = haut ; 5,6 = bas    
                    int rowIndex = isUpperRow ? 1 : 2; // 3/4 sur la première rangée, 5/6 sur la seconde

                    float verticalOffset = rowOffset * rowIndex;
                    float horizontalOffset = 0f;

                    if (isUpperRow)
                        horizontalOffset = isLeft ? -colInset : colInset;

                    var template = isLeft ? chip1 : chip2;
                    var templateRect = isLeft ? chip1Rect : chip2Rect;

                    EnsureChipSlotUI(
                        __instance,
                        allSlots,
                        slotId: slotId,
                        template: template,
                        templateRect: templateRect,
                        horizontalOffset: horizontalOffset,
                        verticalOffset: verticalOffset
                    );
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlots][PlayerUI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }

        /// <summary>
        /// Crée ou réutilise un slot ChipX dans l'UI :
        /// - Si allSlots contient déjà le slot, on le réutilise.
        /// - Sinon, on cherche un GameObject existant dans le parent.
        /// - Sinon, on clone le template.
        /// Dans tous les cas, on repositionne et on réactive le slot
        /// (l'état final actif/inactif est appliqué après dans la boucle principale).
        /// </summary>
        private static void EnsureChipSlotUI(
            uGUI_Equipment manager,
            Dictionary<string, uGUI_EquipmentSlot> allSlots,
            string slotId,
            uGUI_EquipmentSlot template,
            RectTransform templateRect,
            float horizontalOffset,
            float verticalOffset)
        {
            uGUI_EquipmentSlot slot = null;

            if (!allSlots.TryGetValue(slotId, out slot) || slot == null)
            {
                Transform parent = template.transform.parent;
                foreach (Transform child in parent)
                {
                    if (child.name == slotId)
                    {
                        slot = child.GetComponent<uGUI_EquipmentSlot>();
                        if (slot != null)
                            break;
                    }
                }

                if (slot == null)
                {
                    Transform parentTr = template.transform.parent;
                    var cloneGO = UnityEngine.Object.Instantiate(template.gameObject, parentTr);
                    cloneGO.name = slotId;

                    slot = cloneGO.GetComponent<uGUI_EquipmentSlot>();
                    var rect = cloneGO.GetComponent<RectTransform>();

                    slot.slot = slotId;
                    slot.manager = manager;
                    slot.SetActive(true);
                    slot.ClearIcon();

                    allSlots[slotId] = slot;

                    Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Created uGUI slot for '{slotId}'.");
                }
            }

            var slotRect = slot.GetComponent<RectTransform>();

            Vector2 basePos = templateRect.anchoredPosition;
            slotRect.anchoredPosition = new Vector2(
                basePos.x + horizontalOffset,
                basePos.y + verticalOffset
            );

            slot.SetActive(true);
            slot.gameObject.SetActive(true);

            Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Positioned '{slotId}' at {slotRect.anchoredPosition}.");
        }
    }
}

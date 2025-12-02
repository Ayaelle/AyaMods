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
    /// Cette version est plus robuste :
    /// - Réutilise un slot existant si déjà créé (allSlots ou hiérarchie),
    /// - Repositionne et réactive toujours Chip3 / Chip4 à chaque Init().
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
                // ExtraSlots globalement désactivé ?
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

                Log.Info(
                    $"[UtilitySlots][ExtraSlots][PlayerUI] " +
                    $"slotHeight={slotHeight:F1}, rowOffset={rowOffset:F1}, " +
                    $"colInset={colInset:F1}, chip1Pos={p1}, chip2Pos={p2}"
                );

                // Chip3 (gauche, rangée du haut, vers l'extérieur)
                if (desired >= 3)
                {
                    EnsureChipSlotUI(
                        __instance,
                        allSlots,
                        slotId: "Chip3",
                        template: chip1,
                        templateRect: chip1Rect,
                        horizontalOffset: -colInset,
                        verticalOffset: rowOffset
                    );
                }

                // Chip4 (droite, rangée du haut, vers l'extérieur)
                if (desired >= 4)
                {
                    EnsureChipSlotUI(
                        __instance,
                        allSlots,
                        slotId: "Chip4",
                        template: chip2,
                        templateRect: chip2Rect,
                        horizontalOffset: +colInset,
                        verticalOffset: rowOffset
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
        /// Dans tous les cas, on repositionne et on réactive le slot.
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

            // 1) Essayer via le dictionnaire
            if (!allSlots.TryGetValue(slotId, out slot) || slot == null)
            {
                // 2) Essayer de retrouver un éventuel GO déjà présent dans la hiérarchie
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

                // 3) Toujours rien ? On clone le template
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

            // 4) Dans tous les cas, on repositionne et on réactive
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

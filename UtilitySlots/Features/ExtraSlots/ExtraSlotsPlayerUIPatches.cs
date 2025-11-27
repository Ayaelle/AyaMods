using System.Collections.Generic;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Gère l'affichage des slots de puce supplémentaires dans le PDA.
    /// On se cale sur les slots Chip1/Chip2 vanilla et on place :
    ///
    ///     [Chip5]   [Chip6]
    ///    [Chip3] [HEAD] [Chip4]
    ///     [Chip1] [Chip2]
    ///
    /// Les slots 5/6 n'apparaissent que si le slider de ChipSlots le demande.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), "Init")]
    public static class ExtraSlotsPlayerUIPatches
    {
        /// <summary>
        /// Postfix sur uGUI_Equipment.Init : on ajoute/positionne les slots de puce extra.
        /// </summary>
        [HarmonyPostfix]
        public static void Init_Postfix(uGUI_Equipment __instance, Equipment equipment)
        {
            if (!ExtraSlotsRuntime.IsEnabled())
                return;

            if (equipment == null)
                return;

            // Slots internes de uGUI_Equipment
            var allSlotsField = AccessTools.Field(typeof(uGUI_Equipment), "allSlots");
            if (allSlotsField == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Cannot find uGUI_Equipment.allSlots field.");
                return;
            }

            var allSlots = allSlotsField.GetValue(__instance) as Dictionary<string, uGUI_EquipmentSlot>;
            if (allSlots == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] allSlots is null.");
                return;
            }

            // On a besoin au minimum de Chip1 et Chip2 vanilla comme repère
            if (!allSlots.TryGetValue("Chip1", out var chip1Slot) || chip1Slot == null ||
                !allSlots.TryGetValue("Chip2", out var chip2Slot) || chip2Slot == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Chip1/Chip2 slots not found in uGUI_Equipment.");
                return;
            }

            RectTransform chip1Rect = chip1Slot.rectTransform;
            RectTransform chip2Rect = chip2Slot.rectTransform;

            if (chip1Rect == null || chip2Rect == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Chip1/Chip2 rectTransforms are null.");
                return;
            }

            // Parent commun pour tous les slots de puce
            RectTransform parent = chip1Rect.parent as RectTransform;
            if (parent == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlots][PlayerUI] Chip1 parent RectTransform is null.");
                return;
            }

            // Hauteur de slot et espacement vertical
            float slotHeight = Mathf.Abs(chip1Rect.rect.height);
            // Espacement vertical entre chaque rangée
            float rowOffset = slotHeight * 0.9f;   // un peu moins que 1.0 pour éviter de sortir du cadre
            // Décalage horizontal vers l'intérieur pour la rangée du milieu (Chip3/4)
            float colOutset = Mathf.Abs(chip2Rect.anchoredPosition.x - chip1Rect.anchoredPosition.x) * 0.25f;

            Vector2 p1 = chip1Rect.anchoredPosition;
            Vector2 p2 = chip2Rect.anchoredPosition;

            Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] slotHeight={slotHeight:F1}, rowOffset={rowOffset:F1}, colInset={colOutset:F1}, chip1Pos={p1}, chip2Pos={p2}");

            int desiredChips = ExtraSlotsRuntime.GetDesiredPlayerChips();
            if (desiredChips <= 2)
            {
                // Rien à faire, Vanilla only.
                return;
            }

            // ----- Rangée du milieu : Chip3 / Chip4 -----
            // Chip3 : au-dessus de Chip1, vers le centre
            Vector2 chip3Pos = new Vector2(
                p1.x - colOutset,
                p1.y + rowOffset
            );

            // Chip4 : au-dessus de Chip2, vers le centre
            Vector2 chip4Pos = new Vector2(
                p2.x + colOutset,
                p2.y + rowOffset
            );

            // Crée les slots 3/4 si besoin
            if (desiredChips >= 3)
                EnsureChipSlotUI(__instance, allSlots, chip1Slot, parent, "Chip3", chip3Pos);

            if (desiredChips >= 4)
                EnsureChipSlotUI(__instance, allSlots, chip2Slot, parent, "Chip4", chip4Pos);

            // ----- Rangée du haut : Chip5 / Chip6 -----
            if (desiredChips >= 5)
            {
                Vector2 chip5Pos = new Vector2(chip3Pos.x, chip3Pos.y + rowOffset);
                EnsureChipSlotUI(__instance, allSlots, chip1Slot, parent, "Chip5", chip5Pos);
            }

            if (desiredChips >= 6)
            {
                Vector2 chip6Pos = new Vector2(chip4Pos.x, chip4Pos.y + rowOffset);
                EnsureChipSlotUI(__instance, allSlots, chip2Slot, parent, "Chip6", chip6Pos);
            }
        }

        /// <summary>
        /// Crée ou repositionne un slot uGUI pour un ID de chip donné.
        /// </summary>
        private static void EnsureChipSlotUI(
            uGUI_Equipment ui,
            Dictionary<string, uGUI_EquipmentSlot> allSlots,
            uGUI_EquipmentSlot reference,
            RectTransform parent,
            string slotId,
            Vector2 anchoredPos
        )
        {
            if (!allSlots.TryGetValue(slotId, out var slot) || slot == null)
            {
                // On clone le slot de référence (Chip1/Chip2) pour garder le style vanilla
                var cloneGO = Object.Instantiate(reference.gameObject, parent);
                cloneGO.name = $"Slot_{slotId}";

                slot = cloneGO.GetComponent<uGUI_EquipmentSlot>();
                if (slot == null)
                {
                    Log.Warn($"[UtilitySlots][ExtraSlots][PlayerUI] Cloned GameObject for '{slotId}' has no uGUI_EquipmentSlot.");
                    Object.Destroy(cloneGO);
                    return;
                }

                slot.slot = slotId;
                allSlots[slotId] = slot;
                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Created uGUI slot for '{slotId}'.");
            }

            // Activation + position
            slot.SetActive(true);
            var rt = slot.rectTransform;
            if (rt != null)
            {
                rt.SetParent(parent, false);
                rt.anchoredPosition = anchoredPos;
                Log.Info($"[UtilitySlots][ExtraSlots][PlayerUI] Positioned '{slotId}' at {anchoredPos}.");
            }
        }
    }
}

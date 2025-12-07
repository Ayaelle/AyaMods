using System;
using AyaCoreMod.Core;
using UnityEngine;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Gestion UI des slots de modules de l'Exosuit.
    /// On ajoute les slots ExosuitModule5..N dans une grille 2xN sous les modules vanilla.
    /// Les bras (ExosuitArmLeft/Right) restent gérés par le jeu.
    /// </summary>
    internal static class ExtraSlotsVehiclesExosuitUI
    {
        private const float ScaleFactor = 0.8f;
        private static bool _scaledOnce;

        public static void Refresh(uGUI_Equipment ui)
        {
            try
            {
                int desired = ExtraSlotsVehiclesRuntime.GetDesiredExosuitModuleSlots();
                if (desired <= ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots)
                    return;

                var root = ui.transform as RectTransform;
                if (root == null)
                    return;

                var slot1 = root.Find("ExosuitModule1") as RectTransform;
                var slot2 = root.Find("ExosuitModule2") as RectTransform;
                var slot3 = root.Find("ExosuitModule3") as RectTransform;

                if (slot1 == null || slot2 == null)
                    return;

                var templateSlot = slot1.GetComponent<uGUI_EquipmentSlot>();
                if (templateSlot == null)
                    return;

                Transform parent = slot1.parent;
                if (parent == null)
                    return;

                // ---- Mise à l'échelle des slots vanilla (Option B) ----
                if (!_scaledOnce)
                {
                    var newScale = slot1.localScale * ScaleFactor;
                    slot1.localScale = newScale;
                    slot2.localScale = newScale;
                    if (slot3 != null)
                        slot3.localScale = newScale;

                    _scaledOnce = true;
                }

                float slotHeight = Mathf.Abs(slot1.rect.height);
                if (slotHeight <= 0f)
                    slotHeight = 100f;

                float rowOffset;

                if (slot3 != null)
                    rowOffset = Mathf.Abs(slot1.localPosition.y - slot3.localPosition.y);
                else
                    rowOffset = slotHeight * 1.1f;

                if (rowOffset <= 0f)
                    rowOffset = slotHeight * 1.1f;

                float lowestVanillaY = Mathf.Min(
                    slot1.localPosition.y,
                    slot2.localPosition.y,
                    slot3 != null ? slot3.localPosition.y : slot1.localPosition.y
                );

                float startY = lowestVanillaY - rowOffset;

                int createdOrUpdated = 0;

                for (int index = ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1;
                     index <= desired;
                     index++)
                {
                    string slotId = "ExosuitModule" + index;

                    RectTransform slotRT = EnsureSlot(parent, slotId, templateSlot);
                    if (slotRT == null)
                        continue;

                    // Les clones prennent la même scale que les slots vanilla.
                    slotRT.localScale = slot1.localScale;

                    int extraIndex = index - (ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1);
                    int row = extraIndex / 2;
                    bool isLeft = (index % 2) == 1; // 5,7,9,11 à gauche

                    float x = isLeft ? slot1.localPosition.x : slot2.localPosition.x;
                    float y = startY - row * rowOffset;

                    slotRT.localPosition = new Vector3(x, y, slotRT.localPosition.z);

                    createdOrUpdated++;
                }

                Log.Info($"[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Slots créés/mis à jour={createdOrUpdated}, modules={desired}.");
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Exception in Refresh: " + e);
            }
        }

        private static RectTransform EnsureSlot(Transform parent, string slotId, uGUI_EquipmentSlot template)
        {
            var existing = parent.Find(slotId) as RectTransform;
            if (existing != null)
                return existing;

            var go = UnityEngine.Object.Instantiate(template.gameObject, parent, false);
            go.name = slotId;

            var eqSlot = go.GetComponent<uGUI_EquipmentSlot>();
            if (eqSlot == null)
                return null;

            eqSlot.slot = slotId;
            return go.transform as RectTransform;
        }
    }
}

using System;
using AyaCoreMod.Core;
using UnityEngine;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Gestion UI des slots de modules du Cyclops.
    /// On répartit les slots Module1..N sur une grille 3 colonnes x plusieurs lignes,
    /// en réutilisant les positions de Module1..6 comme gabarit.
    /// </summary>
    internal static class ExtraSlotsVehiclesCyclopsUI
    {
        private const float ScaleFactor = 0.8f;
        private static bool _scaledOnce;

        public static void Refresh(uGUI_Equipment ui)
        {
            try
            {
                int desired = ExtraSlotsVehiclesRuntime.GetDesiredCyclopsModuleSlots();
                if (desired <= ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots)
                    return;

                var root = ui.transform as RectTransform;
                if (root == null)
                    return;

                // En vanilla : Module1..6 (3 colonnes x 2 lignes).
                var m1 = root.Find("Module1") as RectTransform;
                var m2 = root.Find("Module2") as RectTransform;
                var m3 = root.Find("Module3") as RectTransform;
                var m4 = root.Find("Module4") as RectTransform;

                if (m1 == null || m2 == null || m3 == null || m4 == null)
                    return;

                var templateSlot = m1.GetComponent<uGUI_EquipmentSlot>();
                if (templateSlot == null)
                    return;

                Transform parent = m1.parent;
                if (parent == null)
                    return;

                // ---- Mise à l'échelle des slots vanilla (Option B) ----
                if (!_scaledOnce)
                {
                    var newScale = m1.localScale * ScaleFactor;
                    m1.localScale = newScale;
                    m2.localScale = newScale;
                    m3.localScale = newScale;
                    m4.localScale = newScale;

                    // Si Module5/6 existent déjà (certains mods), on les scale aussi.
                    var m5 = root.Find("Module5") as RectTransform;
                    var m6 = root.Find("Module6") as RectTransform;
                    if (m5 != null) m5.localScale = newScale;
                    if (m6 != null) m6.localScale = newScale;

                    _scaledOnce = true;
                }

                // Colonnes = X des trois premiers modules.
                float xCol0 = m1.localPosition.x;
                float xCol1 = m2.localPosition.x;
                float xCol2 = m3.localPosition.x;

                float slotHeight = Mathf.Abs(m1.rect.height);
                if (slotHeight <= 0f)
                    slotHeight = 100f;

                // Distance verticale entre la 1ère et 2ème ligne vanilla.
                float rowOffset = Mathf.Abs(m1.localPosition.y - m4.localPosition.y);
                if (rowOffset <= 0f)
                    rowOffset = slotHeight * 1.1f;

                // Y de la première ligne (modules 1..3).
                float firstRowY = (m1.localPosition.y + m2.localPosition.y + m3.localPosition.y) / 3f;

                int createdOrUpdated = 0;

                // On relayout tous les modules 1..desired pour avoir une grille propre.
                for (int index = 1; index <= desired; index++)
                {
                    string slotId = "Module" + index;

                    RectTransform slotRT = EnsureSlot(parent, slotId, templateSlot);
                    if (slotRT == null)
                        continue;

                    // Les clones (et éventuels slots déjà existants) prennent la même scale.
                    slotRT.localScale = m1.localScale;

                    int idx0 = index - 1;      // 0..N-1
                    int row = idx0 / 3;        // 3 colonnes
                    int col = idx0 % 3;

                    float x = col == 0 ? xCol0 : (col == 1 ? xCol1 : xCol2);
                    float y = firstRowY - row * rowOffset;

                    slotRT.localPosition = new Vector3(x, y, slotRT.localPosition.z);

                    createdOrUpdated++;
                }

                Log.Info($"[UtilitySlots][ExtraSlotsVehicles][CyclopsUI] Slots créés/mis à jour={createdOrUpdated}, modules={desired}.");
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][CyclopsUI] Exception in Refresh: " + e);
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

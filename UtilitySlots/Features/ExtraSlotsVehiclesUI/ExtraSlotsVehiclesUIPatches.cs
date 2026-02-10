using System;
using System.Collections.Generic;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    /// <summary>
    /// Même logique que Chip3..Chip6 :
    /// Patch uGUI_Equipment.Init(equipment), clone Module1/2/... et crée ModuleX.
    /// Le “contexte” est déterminé par equipment.owner (SeaMoth / Exosuit / SubRoot via UpgradeConsole).
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), "Init")]
    internal static class ExtraSlotsVehiclesUIPatches
    {
        static void Postfix(uGUI_Equipment __instance, Equipment equipment)
        {
            try
            {
                if (equipment == Inventory.main?.equipment)
                    return;

                if (!ExtraSlotsVehiclesRuntime.IsEnabled()) return;
                if (__instance == null || equipment == null) return;

                // on détecte quel écran c’est via l’owner réel de l’Equipment
                // (dans tes assemblies, Equipment.owner existe)
                var owner = equipment.owner;
                if (owner == null)
                    return;

                if (!ExtraSlotsVehiclesUIBootstrap.TryGetAllSlots(__instance, out var allSlots))
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][UI] allSlots dictionary is null.");
                    return;
                }

                if (owner != null && owner.GetComponent<SeaMoth>() != null)
                {
                    ApplyGrid(__instance, allSlots, prefix: "SeamothModule", vanilla: 4, desired: ExtraSlotsVehiclesRuntime.DesiredSeamothModules());
                }
                else if (owner != null && owner.GetComponent<Exosuit>() != null)
                {
                    ApplyGrid(__instance, allSlots, prefix: "ExosuitModule", vanilla: 4, desired: ExtraSlotsVehiclesRuntime.DesiredExosuitModules());
                }
                else
                {
                    // Cyclops : l’owner des modules est le GameObject du SubRoot (via UpgradeConsole.modules)
                    // On reconnaît juste le prefix "Module" (et vanilla 6)
                    // Si Module1 existe dans allSlots, c’est très probablement cyclops.
                    //if (allSlots.ContainsKey("Module1"))
                        //ApplyGrid(__instance, allSlots, prefix: "Module", vanilla: 6, desired: ExtraSlotsVehiclesRuntime.DesiredCyclopsModules());
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][UI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }

        private static void ApplyGrid(
            uGUI_Equipment manager,
            Dictionary<string, uGUI_EquipmentSlot> allSlots,
            string prefix,
            int vanilla,
            int desired)
        {
            if (desired <= vanilla) return;

            string id1 = $"{prefix}1";
            string id2 = $"{prefix}2";
            string id3 = $"{prefix}3";

            if (!allSlots.TryGetValue(id1, out var s1) || s1 == null ||
                !allSlots.TryGetValue(id2, out var s2) || s2 == null)
            {
                // Pas le bon écran / pas encore construit
                return;
            }

            var r1 = s1.GetComponent<RectTransform>();
            var r2 = s2.GetComponent<RectTransform>();
            if (r1 == null || r2 == null) return;

            // Row spacing : si on a 3, on mesure 1->3, sinon fallback sur hauteur
            float slotH = Mathf.Abs(r1.rect.height);
            float colSpacing = Mathf.Abs(r2.anchoredPosition.x - r1.anchoredPosition.x);
            float rowSpacing = slotH * 0.75f;

            if (allSlots.TryGetValue(id3, out var s3) && s3 != null)
            {
                var r3 = s3.GetComponent<RectTransform>();
                if (r3 != null)
                    rowSpacing = Mathf.Abs(r1.anchoredPosition.y - r3.anchoredPosition.y);
            }

            float scale = ExtraSlotsVehiclesUIBootstrap.ComputeScaleForDesired(desired);

            // On s’aligne sur le mapping vanilla :
            // 1(L haut), 2(R haut), 3(L bas), 4(R bas), puis on continue en rows supplémentaires
            for (int i = vanilla + 1; i <= desired; i++)
            {
                string slotId = $"{prefix}{i}";
                bool isLeft = (i % 2) == 1;
                int rowIndex = (i - 1) / 2; // 1/2 => row0 ; 3/4 => row1 ; 5/6 => row2 ...
                int colIndex = isLeft ? 0 : 1;

                var template = isLeft ? s1 : s2;
                var templateRect = isLeft ? r1 : r2;

                EnsureSlotUI(
                    manager,
                    allSlots,
                    slotId,
                    template,
                    templateRect,
                    colIndex,
                    rowIndex,
                    colSpacing,
                    rowSpacing,
                    scale
                );
            }
        }

        private static void EnsureSlotUI(
            uGUI_Equipment manager,
            Dictionary<string, uGUI_EquipmentSlot> allSlots,
            string slotId,
            uGUI_EquipmentSlot template,
            RectTransform templateRect,
            int colIndex,
            int rowIndex,
            float colSpacing,
            float rowSpacing,
            float scale)
        {
            uGUI_EquipmentSlot slot = null;

            // 1) déjà connu
            if (!allSlots.TryGetValue(slotId, out slot) || slot == null)
            {
                // 2) existe dans la hiérarchie (au cas où)
                Transform parent = template.transform.parent;
                foreach (Transform child in parent)
                {
                    if (child.name == slotId)
                    {
                        slot = child.GetComponent<uGUI_EquipmentSlot>();
                        if (slot != null) break;
                    }
                }

                // 3) sinon clone
                if (slot == null)
                {
                    var cloneGO = UnityEngine.Object.Instantiate(template.gameObject, template.transform.parent);
                    cloneGO.name = slotId;

                    slot = cloneGO.GetComponent<uGUI_EquipmentSlot>();
                    slot.slot = slotId;
                    slot.manager = manager;
                    slot.SetActive(true);
                    slot.ClearIcon();

                    allSlots[slotId] = slot;

                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][UI] Created uGUI slot for '{slotId}'.");
                }
            }

            var rect = slot.GetComponent<RectTransform>();
            if (rect == null) return;

            // Position : on part de la pos du template et on “descend” en rows
            var basePos = templateRect.anchoredPosition;

            // colIndex: 0 => x = x_left ; 1 => x = x_left + colSpacing (ou x_right)
            float x = (colIndex == 0) ? basePos.x : (basePos.x); // basePos correspond déjà au bon côté
            float y = basePos.y - (rowSpacing * rowIndex);

            rect.anchoredPosition = new Vector2(x, y);
            rect.localScale = new Vector3(scale, scale, 1f);

            slot.SetActive(true);
            slot.gameObject.SetActive(true);

            Log.Info($"[UtilitySlots][ExtraSlotsVehicles][UI] Positioned '{slotId}' at {rect.anchoredPosition}, scale={scale:0.00}.");
        }
    }
}

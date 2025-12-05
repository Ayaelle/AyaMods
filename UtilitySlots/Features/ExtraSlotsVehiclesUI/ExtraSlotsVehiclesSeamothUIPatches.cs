using System;
using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// UI PDA pour les modules du Seamoth.
    /// - N'agit que si l'Equipment appartient à un SeaMoth.
    /// - Construit une grille 4×3 en se basant sur SeamothModule1/2/3.
    /// - Ne déplace pas les slots vanilla (1..4), ajoute seulement 5..12.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Init))]
    internal static class ExtraSlotsVehiclesSeamothUIPatches
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

                GameObject ownerGO = equipment.owner;
                if (ownerGO == null)
                    return;

                if (ownerGO.GetComponent<SeaMoth>() == null)
                    return;

                var allSlotsObj = AllSlotsField?.GetValue(__instance);
                var allSlots = allSlotsObj as Dictionary<string, uGUI_EquipmentSlot>;
                if (allSlots == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] allSlots dictionary is null.");
                    return;
                }

                int desired = ExtraSlotsVehiclesRuntime.GetDesiredSeamothModuleSlots();
                if (desired <= ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots)
                    return;

                if (!allSlots.TryGetValue("SeamothModule1", out var mod1) || mod1 == null ||
                    !allSlots.TryGetValue("SeamothModule2", out var mod2) || mod2 == null ||
                    !allSlots.TryGetValue("SeamothModule3", out var mod3) || mod3 == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Could not find vanilla SeamothModule1/2/3.");
                    return;
                }

                var r1 = mod1.GetComponent<RectTransform>();
                var r2 = mod2.GetComponent<RectTransform>();
                var r3 = mod3.GetComponent<RectTransform>();
                if (r1 == null || r2 == null || r3 == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Missing RectTransform on vanilla module slots.");
                    return;
                }

                // On déduit l'espacement horizontal/vertical d'après le layout vanilla
                Vector2 basePos = r1.anchoredPosition;
                float colSpacing = r2.anchoredPosition.x - r1.anchoredPosition.x;
                float rowSpacing = r3.anchoredPosition.y - r1.anchoredPosition.y;

                int createdSlots = 0;

                for (int i = ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots + 1;
                     i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots;
                     i++)
                {
                    // index global 0..11 pour Module1..12
                    int globalIndex = i - 1;
                    int rowIndex = globalIndex / 4;  // 0,1,2
                    int colIndex = globalIndex % 4;  // 0..3

                    // Les slots 1..4 (rowIndex 0) restent vanilla, on ne les touche pas.
                    // Les slots 5..8 -> rowIndex 1, 9..12 -> rowIndex 2.
                    float targetX = basePos.x + colSpacing * colIndex;
                    float targetY = basePos.y + rowSpacing * rowIndex;

                    string slotId = $"SeamothModule{i}";
                    float hOffset = targetX - basePos.x;
                    float vOffset = targetY - basePos.y;

                    EnsureVehicleSlotUI(
                        __instance,
                        allSlots,
                        slotId: slotId,
                        template: mod1,
                        templateRect: r1,
                        horizontalOffset: hOffset,
                        verticalOffset: vOffset
                    );
                    createdSlots++;
                }

                if (createdSlots > 0)
                {
                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Slots créés/mis à jour={createdSlots}, modules={desired}.");
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }

        private static void EnsureVehicleSlotUI(
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
                    var parentTr = template.transform.parent;
                    var cloneGO = UnityEngine.Object.Instantiate(template.gameObject, parentTr);
                    cloneGO.name = slotId;

                    slot = cloneGO.GetComponent<uGUI_EquipmentSlot>();
                    slot.slot = slotId;
                    slot.manager = manager;
                    slot.SetActive(true);
                    slot.ClearIcon();

                    allSlots[slotId] = slot;

                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Created uGUI slot for '{slotId}'.");
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

            Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Positioned '{slotId}' at {slotRect.anchoredPosition}.");
        }
    }
}

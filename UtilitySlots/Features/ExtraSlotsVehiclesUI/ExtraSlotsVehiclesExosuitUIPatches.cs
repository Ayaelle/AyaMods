using AyaCoreMod.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UtilitySlots.Features.ExtraSlotsCore;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// UI PDA pour les modules d'Exosuit.
    /// - N'agit que si l'Equipment appartient à un Exosuit.
    /// - Clone "ExosuitModule1" comme template de slot.
    /// - Crée/positionne ExosuitModule5..N en dessous.
    /// - Crée aussi les écrans "screenExosuitModule5..N" en clonant "screenExosuitModule1"
    ///   pour éviter les logs "slot not found in pda screenExosuitModuleX".
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Init))]
    internal static class ExtraSlotsVehiclesExosuitUIPatches
    {
        private static readonly FieldInfo AllSlotsField =
            AccessTools.Field(typeof(uGUI_Equipment), "allSlots");

        private static readonly FieldInfo OwnerField =
            AccessTools.Field(typeof(Equipment), "owner"); // GameObject

        static void Postfix(uGUI_Equipment __instance, Equipment equipment)
        {
            try
            {
                if (!ExtraSlotsRuntime.IsEnabled())
                    return;

                if (equipment == null)
                    return;

                var ownerGO = OwnerField?.GetValue(equipment) as GameObject;
                if (ownerGO == null)
                    return;

                // On ne touche qu'aux Equipments appartenant à un Exosuit
                if (ownerGO.GetComponent<Exosuit>() == null)
                    return;

                var allSlotsObj = AllSlotsField?.GetValue(__instance);
                var allSlots = allSlotsObj as Dictionary<string, uGUI_EquipmentSlot>;
                if (allSlots == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] allSlots dictionary is null.");
                    return;
                }

                int desired = ExtraSlotsVehiclesRuntime.GetDesiredExosuitModuleSlots();
                if (desired <= ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots)
                    return;

                if (!allSlots.TryGetValue("ExosuitModule1", out var module1) || module1 == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Could not find ExosuitModule1 UI slot.");
                    return;
                }

                var templateRect = module1.GetComponent<RectTransform>();
                if (templateRect == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] ExosuitModule1 RectTransform missing.");
                    return;
                }

                float slotHeight = Mathf.Abs(templateRect.rect.height);
                float rowOffset = slotHeight * 0.9f;

                int createdSlots = 0;

                for (int i = ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1;
                     i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxExosuitModuleSlots;
                     i++)
                {
                    string slotId = $"ExosuitModule{i}";
                    int rowIndex = i - ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots;
                    float verticalOffset = -rowOffset * rowIndex;

                    EnsureVehicleSlotUI(
                        __instance,
                        allSlots,
                        slotId: slotId,
                        template: module1,
                        templateRect: templateRect,
                        horizontalOffset: 0f,
                        verticalOffset: verticalOffset
                    );
                    createdSlots++;
                }

                // Création des "screens" Exosuit pour le PDA
                int createdScreens = EnsureExosuitModuleScreens(__instance, desired);

                if (createdSlots > 0 || createdScreens > 0)
                {
                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Slots créés/mis à jour={createdSlots}, screens créés={createdScreens}, modules={desired}.");
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Exception in uGUI_Equipment.Init postfix: " + e);
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
                    var rect = cloneGO.GetComponent<RectTransform>();

                    slot.slot = slotId;
                    slot.manager = manager;
                    slot.SetActive(true);
                    slot.ClearIcon();

                    allSlots[slotId] = slot;

                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Created uGUI slot for '{slotId}'.");
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

            Log.Info($"[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Positioned '{slotId}' at {slotRect.anchoredPosition}.");
        }

        /// <summary>
        /// Crée les GameObjects "screenExosuitModule5..N" en clonant "screenExosuitModule1"
        /// si besoin, pour éviter les logs "slot not found in pda screenExosuitModuleX".
        /// </summary>
        private static int EnsureExosuitModuleScreens(uGUI_Equipment ui, int desired)
        {
            int created = 0;

            Transform root = ui.transform.root;
            if (root == null)
                return 0;

            // On cherche le template "screenExosuitModule1"
            var templateScreen = FindDeepChild(root, "screenExosuitModule1");
            if (templateScreen == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] screenExosuitModule1 not found; PDA screens will stay vanilla.");
                return 0;
            }

            for (int i = ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots + 1;
                 i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxExosuitModuleSlots;
                 i++)
            {
                string screenName = $"screenExosuitModule{i}";

                var existing = FindDeepChild(root, screenName);
                if (existing != null)
                    continue;

                var cloneGO = UnityEngine.Object.Instantiate(templateScreen.gameObject, templateScreen.parent);
                cloneGO.name = screenName;
                cloneGO.SetActive(templateScreen.gameObject.activeSelf);

                created++;
                Log.Info($"[UtilitySlots][ExtraSlotsVehicles][ExosuitUI] Created PDA screen '{screenName}' (clone of screenExosuitModule1).");
            }

            return created;
        }

        /// <summary>
        /// Recherche récursive de Transform par nom.
        /// </summary>
        private static Transform FindDeepChild(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                    return child;

                var result = FindDeepChild(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}

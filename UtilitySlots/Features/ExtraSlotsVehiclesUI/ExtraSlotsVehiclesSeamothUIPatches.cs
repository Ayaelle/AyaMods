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
    /// UI PDA pour les modules du Seamoth.
    /// - N'agit que si l'Equipment appartient à un SeaMoth.
    /// - Clone "SeamothModule1" comme template pour les slots UI.
    /// - Crée/positionne SeamothModule5..N en dessous.
    /// - Crée aussi les "screenSeamothModule5..N" en clonant "screenSeamothModule1"
    ///   pour éviter les logs "slot not found in pda screenSeamothModuleX".
    /// </summary>
    [HarmonyPatch(typeof(uGUI_Equipment), nameof(uGUI_Equipment.Init))]
    internal static class ExtraSlotsVehiclesSeamothUIPatches
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

                // On ne touche qu'aux Equipments appartenant à un SeaMoth
                if (ownerGO.GetComponent<SeaMoth>() == null)
                    return;

                // Récupérer les slots UI
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

                if (!allSlots.TryGetValue("SeamothModule1", out var module1) || module1 == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Could not find SeamothModule1 UI slot.");
                    return;
                }

                var templateRect = module1.GetComponent<RectTransform>();
                if (templateRect == null)
                {
                    Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] SeamothModule1 RectTransform missing.");
                    return;
                }

                // Layout vertical simple (pour l’instant)
                float slotHeight = Mathf.Abs(templateRect.rect.height);
                float rowOffset = slotHeight * 0.9f;

                int createdSlots = 0;

                for (int i = ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots + 1;
                     i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots;
                     i++)
                {
                    string slotId = $"SeamothModule{i}";
                    int rowIndex = i - ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots;
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

                // Création des "screens" Seamoth pour le PDA
                int createdScreens = EnsureSeamothModuleScreens(__instance, desired);

                if (createdSlots > 0 || createdScreens > 0)
                {
                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Slots créés/mis à jour={createdSlots}, screens créés={createdScreens}, modules={desired}.");
                }
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Exception in uGUI_Equipment.Init postfix: " + e);
            }
        }

        /// <summary>
        /// Crée / réutilise un slot UI de module Seamoth (SeamothModuleX).
        /// </summary>
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

            // 1) Dico
            if (!allSlots.TryGetValue(slotId, out slot) || slot == null)
            {
                // 2) Hiérarchie (au cas où un autre mod l’aurait déjà créé)
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

                // 3) Clone si rien trouvé
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

                    Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Created uGUI slot for '{slotId}'.");
                }
            }

            // 4) Positionnement / activation
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

        /// <summary>
        /// Crée les GameObjects "screenSeamothModule5..N" en clonant "screenSeamothModule1"
        /// pour éviter les logs "slot not found in pda screenSeamothModuleX".
        /// </summary>
        private static int EnsureSeamothModuleScreens(uGUI_Equipment ui, int desired)
        {
            int created = 0;

            Transform root = ui.transform.root;
            if (root == null)
                return 0;

            var templateScreen = FindDeepChild(root, "screenSeamothModule1");
            if (templateScreen == null)
            {
                Log.Warn("[UtilitySlots][ExtraSlotsVehicles][SeamothUI] screenSeamothModule1 not found; PDA screens will stay vanilla.");
                return 0;
            }

            for (int i = ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots + 1;
                 i <= desired && i <= ExtraSlotsVehiclesRuntime.MaxSeamothModuleSlots;
                 i++)
            {
                string screenName = $"screenSeamothModule{i}";

                var existing = FindDeepChild(root, screenName);
                if (existing != null)
                    continue;

                var cloneGO = UnityEngine.Object.Instantiate(templateScreen.gameObject, templateScreen.parent);
                cloneGO.name = screenName;
                cloneGO.SetActive(templateScreen.gameObject.activeSelf);

                created++;
                Log.Info($"[UtilitySlots][ExtraSlotsVehicles][SeamothUI] Created PDA screen '{screenName}' (clone of screenSeamothModule1).");
            }

            return created;
        }

        /// <summary>
        /// Recherche récursive d’un Transform par nom dans la hiérarchie.
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

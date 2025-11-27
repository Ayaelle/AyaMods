using System.Collections.Generic;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Gère des labels numériques (1..12) pour les quickslots, attachés aux icônes.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsUILabels
    {
        private static readonly Dictionary<uGUI_QuickSlots, Text[]> LabelMap = new();

        private static readonly FieldInfo IconsField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "icons");

        /// <summary>
        /// Crée un Text sous chaque icône pour afficher un numéro de slot.
        /// </summary>
        private static void CreateLabels(uGUI_QuickSlots instance)
        {
            if (instance == null || IconsField == null)
                return;

            var icons = IconsField.GetValue(instance) as uGUI_ItemIcon[];
            if (icons == null || icons.Length == 0)
                return;

            if (LabelMap.ContainsKey(instance))
                DestroyLabels(instance);

            var labels = new Text[icons.Length];

            for (int i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                if (icon == null)
                    continue;

                // Container sous l'icône
                var iconRect = icon.rectTransform;
                var labelGO = new GameObject($"QuickSlots_Label_{i + 1}")
                {
                    layer = icon.gameObject.layer
                };
                labelGO.transform.SetParent(iconRect, false);

                var rect = labelGO.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, -10f);
                rect.sizeDelta = new Vector2(32f, 16f);

                var text = labelGO.AddComponent<Text>();
                text.text = (i + 1).ToString();
                text.alignment = TextAnchor.MiddleCenter;
                text.fontSize = 14;

                // Font par défaut Unity
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

                text.color = new Color(1f, 1f, 1f, 0.8f);
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                text.raycastTarget = false;

                labelGO.SetActive(RuntimeConfig.ShowQuickSlotLabels);

                labels[i] = text;
            }

            LabelMap[instance] = labels;

            Log.Info($"[UtilitySlots][Quickslots][UI] Labels créés pour {labels.Length} slots.");
        }

        private static void DestroyLabels(uGUI_QuickSlots instance)
        {
            if (instance == null)
                return;

            if (!LabelMap.TryGetValue(instance, out var labels))
                return;

            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label != null)
                {
                    Object.Destroy(label.gameObject);
                }
            }

            LabelMap.Remove(instance);

            Log.Info("[UtilitySlots][Quickslots][UI] Labels détruits.");
        }

        internal static void UpdateLabelVisibility(uGUI_QuickSlots instance)
        {
            if (!LabelMap.TryGetValue(instance, out var labels))
                return;

            bool visible = RuntimeConfig.ShowQuickSlotLabels;

            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (label != null && label.gameObject != null)
                {
                    label.gameObject.SetActive(visible);
                }
            }
        }

        [HarmonyPatch(typeof(uGUI_QuickSlots), "Init")]
        [HarmonyPostfix]
        private static void Init_Postfix(uGUI_QuickSlots __instance)
        {
            CreateLabels(__instance);
        }

        [HarmonyPatch(typeof(uGUI_QuickSlots), "Uninit")]
        [HarmonyPostfix]
        private static void Uninit_Postfix(uGUI_QuickSlots __instance)
        {
            DestroyLabels(__instance);
        }
    }
}
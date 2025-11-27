using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Patch UI sur uGUI_QuickSlots.Update :
    /// - masque/affiche les slots vides selon HideEmptyQuickSlots
    /// - synchronise la visibilité des labels numériques avec ShowQuickSlotLabels
    /// - si on est en train de drag un item bindable, tous les slots restent visibles.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_QuickSlots), "Update")]
    public static class QuickSlotsUIPatches
    {
        private static readonly FieldInfo IconsField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "icons");

        private static readonly FieldInfo BackgroundsField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "backgrounds");

        private static readonly FieldInfo TargetField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "target");

        private static bool _loggedOnce;

        [HarmonyPostfix]
        public static void Update_Postfix(uGUI_QuickSlots __instance)
        {
            if (__instance == null)
                return;

            // Si la feature quickslots est désactivée : on ne touche pas à l'UI
            // (à part la visibilité des labels, pour rester cohérent avec l’option).
            if (!RuntimeConfig.EnableQuickSlots)
            {
                QuickSlotsUILabels.UpdateLabelVisibility(__instance);
                return;
            }

            if (IconsField == null)
                return;

            var icons = IconsField.GetValue(__instance) as uGUI_ItemIcon[];
            if (icons == null || icons.Length == 0)
                return;

            var target = TargetField?.GetValue(__instance) as IQuickSlots;
            if (target == null)
                return;

            var backgrounds = BackgroundsField?.GetValue(__instance) as Image[];

            if (!_loggedOnce)
            {
                _loggedOnce = true;
                Log.Info($"[UtilitySlots][Quickslots][UI] Update_Postfix actif. Icons={icons.Length}");
            }

            // Base: on utilise HideEmptyQuickSlots
            bool effectiveHideEmpty = RuntimeConfig.HideEmptyQuickSlots;

            // Si on est en drag & drop d’un item bindable, on force l’affichage de tous les slots
            if (ItemDragManager.isDragging)
            {
                var draggedItem = ItemDragManager.draggedItem;
                if (draggedItem != null && Inventory.main != null)
                {
                    if (Inventory.main.GetCanBindItem(draggedItem))
                    {
                        effectiveHideEmpty = false;
                    }
                }
            }

            for (int i = 0; i < icons.Length; i++)
            {
                var icon = icons[i];
                if (icon == null)
                    continue;

                var go = icon.gameObject;
                if (go == null)
                    continue;

                bool hasItem = target.GetSlotBinding(i) != TechType.None;

                if (effectiveHideEmpty)
                {
                    go.SetActive(hasItem);

                    if (backgrounds != null && i < backgrounds.Length && backgrounds[i] != null)
                        backgrounds[i].gameObject.SetActive(hasItem);
                }
                else
                {
                    go.SetActive(true);

                    if (backgrounds != null && i < backgrounds.Length && backgrounds[i] != null)
                        backgrounds[i].gameObject.SetActive(true);
                }
            }

            // Toujours synchroniser les labels avec l'option ShowQuickSlotLabels
            QuickSlotsUILabels.UpdateLabelVisibility(__instance);
        }
    }
}
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Ajuste la position des quickslots pour les centrer en fonction
    /// du nombre de slots VISIBLES (GetSlotCount patché côté QuickSlots).
    /// On ne touche pas au reste de l'UI, seulement au X des icônes.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_QuickSlots), "GetPosition")]
    public static class QuickSlotsUISizePatch
    {
        private static readonly FieldInfo TargetField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "target");

        /// <summary>
        /// Postfix sur uGUI_QuickSlots.GetPosition(int slotID)
        /// </summary>
        static void Postfix(uGUI_QuickSlots __instance, int slotID, ref Vector2 __result)
        {
            // Si le mod QuickSlots est désactivé, on laisse 100% vanilla.
            if (!RuntimeConfig.EnableQuickSlots)
                return;

            if (__instance == null || TargetField == null)
                return;

            var target = TargetField.GetValue(__instance) as IQuickSlots;
            if (target == null)
                return;

            // On veut le QuickSlots concret pour appeler GetSlotCount(),
            // qui est patché dans QuickSlotsLogicPatches.
            var quickSlots = target as QuickSlots;
            if (quickSlots == null)
                return;

            int visibleCount;
            try
            {
                visibleCount = quickSlots.GetSlotCount(); // valeur logique (1..12)
            }
            catch
            {
                return;
            }

            if (visibleCount <= 0)
                return;

            // Si l'index est au-delà de la plage visible, on laisse la position vanilla.
            // (Ces slots seront de toute façon masqués / inutilisés.)
            if (slotID < 0 || slotID >= visibleCount)
                return;

            // Paramètres de base : même step que vanilla (largeur icône + espace)
            const float iconWidth = 58f;
            const float spacing = 8f;
            float step = iconWidth + spacing; // 66f

            // On centre les "visibleCount" premiers slots autour de x = 0.
            float half = 0.5f * (visibleCount - 1);
            float x = (-half + slotID) * step;

            __result = new Vector2(x, __result.y);
        }
    }
}

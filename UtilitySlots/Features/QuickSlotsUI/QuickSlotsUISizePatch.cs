using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Ajuste la position des quickslots.
    /// Version actuelle : reproduit le comportement vanilla (pas de compression),
    /// juste pour garder un point d'accroche si on veut tweaker plus tard.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_QuickSlots), "GetPosition")]
    public static class QuickSlotsUISizePatch
    {
        private static readonly FieldInfo IconsField =
            AccessTools.Field(typeof(uGUI_QuickSlots), "icons");

        static void Postfix(uGUI_QuickSlots __instance, int slotID, ref Vector2 __result)
        {
            if (__instance == null || IconsField == null)
                return;

            var icons = IconsField.GetValue(__instance) as uGUI_ItemIcon[];
            if (icons == null || icons.Length == 0)
                return;

            int count = icons.Length;
            if (count <= 0)
                return;

            // Valeurs vanilla : iconStep.x = 58f, espace = 8f → step = 66f
            const float baseStep = 58f + 8f;
            float half = 0.5f * (count - 1);
            float x = -half * baseStep + slotID * baseStep;

            __result = new Vector2(x, 0f);
        }
    }
}

using System;
using TMPro;
using UnityEngine;

namespace AyaCoreMod.UtilitySlots.UI
{
    public static class UiHelpers
    {
        public static TextMeshProUGUI CreateText(Transform parent, string name, string text, Vector2 anchoredPos, Color color)
        {
            var t = Object.Instantiate(HandReticle.main.compTextHand);
            t.name = name;
            t.transform.SetParent(parent, false);
            t.text = text;
            t.fontSize = 17;
            t.color = color;
            t.alignment = TextAlignmentOptions.Center;
            t.raycastTarget = false;

            var cg = t.GetComponent<CanvasGroup>() ?? t.gameObject.AddComponent<CanvasGroup>();
            cg.blocksRaycasts = false;

            t.rectTransform.anchoredPosition = anchoredPos;
            return t;
        }
    }
}

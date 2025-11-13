using TMPro;
using UnityEngine;

namespace UtilitySlots.UI
{
    /// <summary>
    /// Helper pour créer des textes UI basés sur le prefab du HandReticle,
    /// avec les bonnes options (pas de raycasts, bon parent, etc.).
    /// </summary>
    public static class UiHelpers
    {
        public static TextMeshProUGUI CreateText(Transform parent, string name, string text, Vector2 anchoredPos, Color color)
        {
            var template = HandReticle.main != null ? HandReticle.main.compTextHand : null;
            if (template == null)
                return null;

            var t = Object.Instantiate(template);
            t.name = name;
            t.transform.SetParent(parent, false);
            t.text = text;
            t.fontSize = 17f;
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

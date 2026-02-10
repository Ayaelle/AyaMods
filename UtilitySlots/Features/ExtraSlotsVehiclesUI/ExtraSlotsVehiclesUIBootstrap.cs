using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlotsVehicles
{
    internal static class ExtraSlotsVehiclesUIBootstrap
    {
        // Accès à uGUI_Equipment.allSlots (privé)
        public static readonly System.Reflection.FieldInfo AllSlotsField =
            AccessTools.Field(typeof(uGUI_Equipment), "allSlots");

        public static GameObject FindChildRecursive(Transform root, string name)
        {
            if (root == null) return null;

            foreach (Transform t in root)
            {
                if (t.name == name)
                    return t.gameObject;

                var r = FindChildRecursive(t, name);
                if (r != null) return r;
            }
            return null;
        }

        public static bool TryGetAllSlots(uGUI_Equipment ui, out Dictionary<string, uGUI_EquipmentSlot> allSlots)
        {
            allSlots = null;
            if (ui == null) return false;

            var obj = AllSlotsField?.GetValue(ui);
            allSlots = obj as Dictionary<string, uGUI_EquipmentSlot>;
            return allSlots != null;
        }

        public static float ComputeScaleForDesired(int desired)
        {
            // “no scroll”, on compacte un peu quand on dépasse beaucoup.
            if (desired >= 12) return 0.80f;
            if (desired >= 10) return 0.85f;
            if (desired >= 8) return 0.90f;
            return 1.00f;
        }
    }
}

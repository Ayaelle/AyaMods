using System;
using AyaCoreMod.Core;
using UnityEngine;

namespace UtilitySlots.Features.ExtraSlotsVehiclesUI
{
    /// <summary>
    /// Helpers communs pour la gestion de l'UI des véhicules (recherche récursive, clonage, etc).
    /// </summary>
    internal static class ExtraSlotsVehiclesUIBootstrap
    {
        /// <summary>
        /// Recherche récursive d'un enfant par nom dans la hiérarchie.
        /// </summary>
        public static Transform FindChildRecursive(Transform root, string name)
        {
            if (root == null || string.IsNullOrEmpty(name))
                return null;

            if (root.name == name)
                return root;

            for (int i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                var found = FindChildRecursive(child, name);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Clone un écran de slot (par ex. screenSeamothModule1) sous le même parent, avec un nouveau nom.
        /// </summary>
        public static GameObject CloneScreen(GameObject template, Transform parent, string newName)
        {
            if (template == null || parent == null)
                return null;

            var clone = UnityEngine.Object.Instantiate(template, parent, worldPositionStays: false);
            clone.name = newName;
            clone.SetActive(true);
            return clone;
        }

        /// <summary>
        /// Log utilitaire pour l'UI des véhicules.
        /// </summary>
        public static void LogInfo(string message)
        {
            Log.Info("[UtilitySlots][ExtraSlotsVehicles][UI] " + message);
        }

        public static void LogWarn(string message)
        {
            Log.Warn("[UtilitySlots][ExtraSlotsVehicles][UI] " + message);
        }

        public static void LogError(string message)
        {
            Log.Error("[UtilitySlots][ExtraSlotsVehicles][UI] " + message);
        }
    }
}

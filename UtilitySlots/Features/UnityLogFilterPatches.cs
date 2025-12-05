using System;
using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Features.ExtraSlotsVehciles;

namespace UtilitySlots.Features
{
    /// <summary>
    /// Filtre ultra-ciblé sur UnityEngine.Debug.Log pour supprimer
    /// uniquement les messages "slot not found in pda screenX" qui
    /// concernent NOS slots supplémentaires de véhicules.
    /// </summary>
    [HarmonyPatch]
    internal static class UnityLogFilterPatches
    {
        // Patch Debug.Log(object message)
        [HarmonyPatch(typeof(Debug), nameof(Debug.Log), new[] { typeof(object) })]
        [HarmonyPrefix]
        private static bool DebugLog_Prefix(object message)
        {
            try
            {
                if (message is not string s)
                    return true;

                if (!s.StartsWith("slot not found in pda ", StringComparison.Ordinal))
                    return true;

                int idx = s.IndexOf("screen", StringComparison.Ordinal);
                if (idx < 0)
                    return true;

                string screenName = s.Substring(idx); // ex: "screenSeamothModule12"

                if (IsOurExtraPdaScreenLog(screenName))
                {
                    // On bloque ce log spécifique.
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error("[UtilitySlots][LogFilter] Exception in DebugLog_Prefix: " + e);
                return true;
            }
        }

        private static bool IsOurExtraPdaScreenLog(string screenName)
        {
            // Seamoth : "screenSeamothModule5..12"
            const string seamothPrefix = "screenSeamothModule";
            if (screenName.StartsWith(seamothPrefix, StringComparison.Ordinal))
            {
                if (int.TryParse(screenName.Substring(seamothPrefix.Length), out int idx))
                    return idx > ExtraSlotsVehiclesRuntime.VanillaSeamothModuleSlots;
            }

            // Exosuit : "screenExosuitModule5..12"
            const string exoPrefix = "screenExosuitModule";
            if (screenName.StartsWith(exoPrefix, StringComparison.Ordinal))
            {
                if (int.TryParse(screenName.Substring(exoPrefix.Length), out int idx))
                    return idx > ExtraSlotsVehiclesRuntime.VanillaExosuitModuleSlots;
            }

            // Cyclops : "screenModule7..14"
            const string cyclopsPrefix = "screenModule";
            if (screenName.StartsWith(cyclopsPrefix, StringComparison.Ordinal))
            {
                if (int.TryParse(screenName.Substring(cyclopsPrefix.Length), out int idx))
                    return idx > ExtraSlotsVehiclesRuntime.VanillaCyclopsModuleSlots;
            }

            return false;
        }
    }
}

using HarmonyLib;
using System;
using System.Reflection;

namespace AyaCoreMod.Core
{
    // Utilitaire pour appliquer tous les patches Harmony d'un assembly.
    public static class PatchManager
    {
        public static void ApplyAll(Harmony harmony, Assembly assembly)
        {
            try
            {
                harmony.PatchAll(assembly);
                Log.Info("[AyaCoreMod] Harmony patches applied.");
            }
            catch (Exception ex)
            {
                Log.Error("[AyaCoreMod] PatchAll failed : " + ex);
            }
        }
    }
}

using HarmonyLib;
using System;
using System.Reflection;

namespace AyaMods.Core
{
    public static class PatchManager
    {
        public static void ApplyAll(Harmony h, Assembly asm)
        {
            try { h.PatchAll(asm); Log.Info("[AyaMods.Core] Harmony patches applied."); }
            catch (Exception ex) { Log.Error("[AyaMods.Core] PatchAll failled: " + ex); }
        }
    }
}

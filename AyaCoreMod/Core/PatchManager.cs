using HarmonyLib;
using System;
using System.Reflection;

namespace AyaCoreMod.Core
{
    public static class PatchManager
    {
        public static void ApplyAll(Harmony h, Assembly asm)
        {
            try { h.PatchAll(asm); Log.Info("[AyaCoreMod.Core] Harmony patches applied."); }
            catch (Exception ex) { Log.Error("[AyaCoreMod.Core] PatchAll failled: " + ex); }
        }
    }
}

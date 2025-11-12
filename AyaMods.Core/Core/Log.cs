using BepInEx.Logging;

namespace AyaMods.Core
{
    public static class Log
    {
        static ManualLogSource _l;
        public static void Bind(ManualLogSource l) => _l = l;
        public static void Info(string m) => _l?.LogInfo(m);
        public static void Warn(string m) => _l?.LogWarning(m);
        public static void Error(string m) => _l?.LogError(m);
    }
}

using BepInEx.Logging;

namespace AyaCoreMod.Core
{
    // Petit wrapper autour du logger BepInEx pour pouvoir l'utiliser partout dans le core.
    public static class Log
    {
        private static ManualLogSource _logger;

        public static void Bind(ManualLogSource logger)
        {
            _logger = logger;
        }

        public static void Info(string message) => _logger?.LogInfo(message);
        public static void Warn(string message) => _logger?.LogWarning(message);
        public static void Error(string message) => _logger?.LogError(message);
    }
}

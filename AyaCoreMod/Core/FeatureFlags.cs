namespace AyaCoreMod.Core
{
    /// <summary>
    /// Indicateurs globaux pour activer / désactiver certains comportements du core.
    /// </summary>
    public static class FeatureFlags
    {
        /// <summary>
        /// Quand vrai, aucun patch ni hook ne devrait s'exécuter dans les mods qui respectent ce flag.
        /// Pratique comme "panic mode" en cas de bug sur une save.
        /// </summary>
        public static bool SafeMode = false;
    }
}

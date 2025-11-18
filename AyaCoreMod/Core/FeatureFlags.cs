namespace AyaCoreMod.Core
{
    // Indicateurs globaux pour activer / désactiver certains comportements du core.
    public static class FeatureFlags
    {
        // Quand vrai, aucun patch ni hook ne devrait s'exécuter dans les mods qui respectent ce flag.
        // Pratique comme "panic mode" en cas de bug sur une save.
        public static bool SafeMode = false;
    }
}

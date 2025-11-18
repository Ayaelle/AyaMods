using UnityEngine;

namespace UtilitySlots.Config
{
    /// <summary>
    /// Copie en mémoire des options d'accès interne, tenue à jour en live
    /// par la classe Options. Le gameplay lit uniquement ces valeurs.
    /// </summary>
    public static class RuntimeInternalAccessConfig
    {
        // Toggle global
        public static bool EnableInternalAccess;

        // Toggles détaillés
        public static bool SeamothInternalUpgrades;
        public static bool SeamothInternalStorage;

        public static bool ExosuitInternalUpgrades;
        public static bool ExosuitInternalStorage;

        // Touches
        public static KeyCode InternalUpgradesKey;
        public static KeyCode InternalStorageKey;

        /// <summary>
        /// Applique tout le contenu de l'instance Options courante
        /// dans la config runtime. Appelé au chargement initial.
        /// </summary>
        public static void ApplyFrom(Options o)
        {
            if (o == null)
                return;

            EnableInternalAccess = o.EnableInternalAccess;
            SeamothInternalUpgrades = o.SeamothInternalUpgrades;
            SeamothInternalStorage = o.SeamothInternalStorage;
            ExosuitInternalUpgrades = o.ExosuitInternalUpgrades;
            ExosuitInternalStorage = o.ExosuitInternalStorage;
            InternalUpgradesKey = o.InternalUpgradesKey;
            InternalStorageKey = o.InternalStorageKey;
        }
    }
}

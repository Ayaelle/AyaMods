using System;
using System.Reflection;
using AyaCoreMod.Core;
using HarmonyLib;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Étend le nombre de slots d'upgrade du Seamoth
    /// en modifiant le tableau statique SeaMoth._slotIDs.
    /// Vanilla : 4 slots ("SeamothModule1" .. "SeamothModule4").
    /// ExtraSlots : 4 à 12 slots selon GlobalOptions.SeamothSlots.
    /// </summary>
    [HarmonyPatch(typeof(SeaMoth))]
    public static class ExtraSlotsSeamothPatches
    {
        private static readonly FieldInfo SlotIdsField =
            AccessTools.Field(typeof(SeaMoth), "_slotIDs");

        private static bool _initialized;

        /// <summary>
        /// On se greffe sur SeaMoth.Awake pour être sûr que le type
        /// est chargé, puis on redéfinit le tableau _slotIDs.
        /// </summary>
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void SeaMoth_Awake_Postfix()
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            if (SlotIdsField == null)
            {
                Log.Error("[UtilitySlots][ExtraSlots][Seamoth] Impossible de trouver SeaMoth._slotIDs via reflection.");
                return;
            }

            // On fait l'init une seule fois, inutile de recréer le tableau à chaque Awake.
            if (_initialized)
                return;

            int desiredSlots = gopt.SeamothSlots;

            // On s'assure de ne jamais descendre sous le vanilla (4).
            if (desiredSlots < 4)
                desiredSlots = 4;
            if (desiredSlots > 12)
                desiredSlots = 12;

            try
            {
                string[] newSlots = new string[desiredSlots];
                for (int i = 0; i < desiredSlots; i++)
                {
                    int index = i + 1; // 1-based dans les IDs
                    newSlots[i] = $"SeamothModule{index}";
                }

                SlotIdsField.SetValue(null, newSlots);

                _initialized = true;

                Log.Info(
                    $"[UtilitySlots][ExtraSlots][Seamoth] SeaMoth._slotIDs redéfini à {desiredSlots} slots."
                );
            }
            catch (Exception e)
            {
                Log.Error($"[UtilitySlots][ExtraSlots][Seamoth] Erreur lors de la redéfinition des slots : {e}");
            }
        }
    }
}

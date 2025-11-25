using System;
using HarmonyLib;
using AyaCoreMod.Core;
using UtilitySlots.Config;

namespace UtilitySlots.Features.ExtraSlots
{
    /// <summary>
    /// Étend le nombre de slots Chip du joueur.
    /// </summary>
    [HarmonyPatch(typeof(Equipment))]
    public static class ExtraSlotsPlayerPatches
    {
        /// <summary>
        /// Patch sur Equipment.Awake.
        /// On ne modifie que l'Equipment du joueur.
        /// </summary>
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        private static void Equipment_Awake_Postfix(Equipment __instance)
        {
            var gopt = GlobalOptions.Instance;
            if (gopt == null || !gopt.EnableExtraSlots)
                return;

            // On ne modifie que l'Equipment du joueur
            if (__instance != Inventory.main?.equipment)
                return;

            int desiredChips = gopt.ChipSlots;

            // Vanilla : 4 slots. On ne descend pas en dessous.
            if (desiredChips <= 4)
            {
                Log.Info($"[UtilitySlots][ExtraSlots][Player] Chip slots = {desiredChips} (<= 4), aucun slot supplémentaire ajouté.");
                return;
            }

            Log.Info($"[UtilitySlots][ExtraSlots][Player] Tentative d'extension des chip slots à {desiredChips}.");

            // Ajout des slots Chip5, Chip6, etc.
            for (int i = 5; i <= desiredChips; i++)
            {
                string slotID = $"Chip{i}";

                try
                {
                    Log.Info($"[UtilitySlots][ExtraSlots][Player] Ajout du slot : {slotID}");
                    __instance.AddSlot(slotID);
                }
                catch (Exception e)
                {
                    // Si un autre mod ou le jeu a déjà ce slot, on ne veut pas crasher.
                    Log.Warn($"[UtilitySlots][ExtraSlots][Player] Impossible d'ajouter le slot {slotID} (probablement déjà existant) : {e.Message}");
                }
            }
        }
    }
}

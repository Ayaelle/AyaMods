using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsCore
{
    public enum QuickSlotsContext
    {
        OnFoot,
        Vehicle
    }

    /// <summary>
    /// Logique de calcul des nombres de slots selon la config.
    /// Désormais on ne gère plus activement le contexte Vehicle :
    /// seule la valeur OnFootQuickslots est utilisée.
    /// </summary>
    public static class QuickSlotsRuntime
    {
        public const int HardMaxSlots = 12;

        /// <summary>
        /// Retourne le contexte courant.
        /// On renvoie toujours OnFoot pour simplifier la logique :
        /// les véhicules restent 100% vanilla côté QuickSlots.
        /// </summary>
        public static QuickSlotsContext GetCurrentContext()
        {
            return QuickSlotsContext.OnFoot;
        }

        /// <summary>
        /// Nombre de slots souhaités, limité au hard cap.
        /// On ne différencie plus OnFoot / Vehicle : seule la valeur OnFootQuickslots est utilisée.
        /// </summary>
        public static int GetConfiguredSlots(QuickSlotsContext context)
        {
            // Si la feature runtime est désactivée, on retombe sur le comportement vanilla.
            if (!RuntimeConfig.EnableQuickSlots)
                return 5; // valeur vanilla pour le joueur

            // On ne gère plus explicitement Vehicle ici : on utilise toujours la config "OnFoot".
            int requested = RuntimeConfig.OnFootQuickslots;

            return Mathf.Clamp(requested, 1, HardMaxSlots);
        }

        /// <summary>
        /// Nombre de slots "physiques" maximum à créer dans QuickSlots.
        /// On laisse QuickSlots toujours initialisé avec 12 slots physiques,
        /// et on adapte par-dessus avec GetSlotCount côté patches.
        /// </summary>
        public static int GetPhysicalSlots()
        {
            return HardMaxSlots;
        }
    }
}
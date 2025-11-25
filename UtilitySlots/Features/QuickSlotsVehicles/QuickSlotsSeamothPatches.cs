using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsVehicles
{
    /// <summary>
    /// Patches liés au Seamoth pour la future intégration des bras
    /// dans la barre de quickslots.
    /// Actuellement, ce fichier ne fait rien, il sert de point d'ancrage.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsSeamothPatches
    {
        // Exemple de point d'accroche futur :
        // [HarmonyPatch(typeof(SeaMoth), "Update")]
        // static void Postfix(SeaMoth __instance) { ... }

        static QuickSlotsSeamothPatches()
        {
            Log.Info("[UtilitySlots][Quickslots][Vehicles] Seamoth patches stub initialised.");
        }
    }
}

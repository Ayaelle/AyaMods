using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsVehicles
{
    /// <summary>
    /// Patches liés à l'Exosuit (Prawn) pour la future intégration des bras
    /// dans la barre de quickslots.
    /// Actuellement, ce fichier est un stub.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsExosuitPatches
    {
        // Exemple de point d'accroche futur :
        // [HarmonyPatch(typeof(Exosuit), "Update")]
        // static void Postfix(Exosuit __instance) { ... }

        static QuickSlotsExosuitPatches()
        {
            Log.Info("[UtilitySlots][Quickslots][Vehicles] Exosuit patches stub initialised.");
        }
    }
}

using AyaCoreMod.Core;
using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsCyclops
{
    /// <summary>
    /// Base pour les futurs patches Cyclops (par ex. affichage des quickslots
    /// pour le véhicule amarré dans la console du Cyclops).
    /// Actuellement, ce fichier ne contient que un stub.
    /// </summary>
    [HarmonyPatch]
    public static class QuickSlotsCyclopsPatches
    {
        // Exemple de futur patch :
        // [HarmonyPatch(typeof(SubRoot), "OnDockedChanged")]
        // static void Postfix(SubRoot __instance) { ... }

        static QuickSlotsCyclopsPatches()
        {
            Log.Info("[UtilitySlots][Quickslots][Cyclops] Cyclops patches stub initialised.");
        }
    }
}

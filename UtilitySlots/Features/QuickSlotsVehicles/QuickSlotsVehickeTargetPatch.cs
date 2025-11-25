using HarmonyLib;

namespace UtilitySlots.Features.QuickSlotsVehicles
{
    /// <summary>
    /// Patch neutralisé : on laisse uGUI_QuickSlots.GetTarget fonctionner en vanilla.
    /// (Plus de forçage de quickslots en véhicule pour l'instant.)
    /// </summary>
    [HarmonyPatch(typeof(uGUI_QuickSlots), "GetTarget")]
    public static class QuickSlotsVehiclesTargetPatch
    {
        // Retourne true => Harmony laisse la méthode d'origine s'exécuter normalement.
        static bool Prefix()
        {
            return true;
        }
    }
}

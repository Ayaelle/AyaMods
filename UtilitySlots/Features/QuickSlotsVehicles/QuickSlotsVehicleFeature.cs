using AyaCoreMod.Core;
using AyaCoreMod.Features;

namespace UtilitySlots.Features.QuickSlotsVehicles
{
    /// <summary>
    /// Point d'entrée pour les extensions QuickSlots liées aux véhicules.
    /// Pour l'instant, c'est un stub : la logique spécifique (bras Seamoth/Prawn, etc.)
    /// sera ajoutée dans des patches dédiés.
    /// </summary>
    public class QuickSlotsVehicleFeature : IFeature
    {
        public void Enable()
        {
            Log.Info("[UtilitySlots][Quickslots][Vehicles] Vehicle feature enabled (stub).");
        }

        public void Disable()
        {
            Log.Info("[UtilitySlots][Quickslots][Vehicles] Vehicle feature disabled.");
        }
    }
}

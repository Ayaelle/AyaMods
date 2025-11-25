using AyaCoreMod.Core;
using AyaCoreMod.Features;

namespace UtilitySlots.Features.QuickSlotsCyclops
{
    /// <summary>
    /// Point d'entrée pour la compatibilité Cyclops (console docked, HUD).
    /// Actuellement, la logique est volontairement minimale.
    /// </summary>
    public class QuickSlotsCyclopsFeature : IFeature
    {
        public void Enable()
        {
            Log.Info("[UtilitySlots][Quickslots][Cyclops] Cyclops feature enabled (stub).");
        }

        public void Disable()
        {
            Log.Info("[UtilitySlots][Quickslots][Cyclops] Cyclops feature disabled.");
        }
    }
}

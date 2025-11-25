using AyaCoreMod.Core;
using AyaCoreMod.Features;

namespace UtilitySlots.Features.QuickSlotsKeybinds
{
    /// <summary>
    /// Feature vide pour aligner avec l'architecture (tout se fait via Harmony).
    /// Tu peux l'enregistrer comme les autres features UtilitySlots.
    /// </summary>
    public class QuickSlotsKeybindsFeature : IFeature
    {
        public void Enable()
        {
            Log.Info("[UtilitySlots][Quickslots] QuickSlots Keybinds enabled");
        }

        public void Disable()
        {
            Log.Info("[UtilitySlots][Quickslots] QuickSlots Keybinds disabled");
        }
    }
}

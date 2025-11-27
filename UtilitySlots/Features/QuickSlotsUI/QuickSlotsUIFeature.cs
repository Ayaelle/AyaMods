using AyaCoreMod.Core;
using AyaCoreMod.Features;

namespace UtilitySlots.Features.QuickSlotsUI
{
    /// <summary>
    /// Feature d’enregistrement pour les patchs UI des QuickSlots.
    /// 
    /// Cette feature ne fait rien elle-même : tout est appliqué via Harmony.
    /// Elle sert uniquement à permettre au système UtilitySlots de l’activer
    /// ou désactiver via FeatureRegistry, selon les options globales.
    /// </summary>
    public class QuickSlotsUIFeature : IFeature
    {
        public void Enable()
        {
            Log.Info("[UtilitySlots][Quickslots] QuickSlots UI enabled");
        }

        public void Disable()
        {
            Log.Info("[UtilitySlots][Quickslots] QuickSlots UI disabled");
        }
    }
}
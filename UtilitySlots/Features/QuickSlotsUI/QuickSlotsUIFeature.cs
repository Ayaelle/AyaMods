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
            // Rien à faire : les patchs Harmony sont appliqués au chargement.
        }

        public void Disable()
        {
            // Rien à faire ici non plus : UtilitySlots ne supporte pas encore
            // la dépatch Harmony dynamique — on coupe juste l’enregistrement.
        }
    }
}

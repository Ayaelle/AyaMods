using AyaCoreMod.Features;

namespace UtilitySlots.Features.QuickslotExtensionFeature
{
    /// <summary>
    /// Point d'entrée de la feature d'extension des quickslots.
    /// La logique concrète sera portée par des patches Harmony
    /// que l'on ajoutera plus tard (QuickSlots / uGUI_QuickSlots).
    /// </summary>
    public class QuickslotExtensionFeature : IFeature
    {
        public void Enable()
        {
            // Pour l'instant, rien ici.
            // Les patches Harmony seront appliqués automatiquement via PatchManager.
        }

        public void Disable()
        {
            // Même remarque que pour ExtraSlotsFeature.
        }
    }
}

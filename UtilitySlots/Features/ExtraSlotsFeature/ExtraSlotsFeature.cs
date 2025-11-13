using AyaCoreMod.Features;

namespace UtilitySlots.Features.ExtraSlotsFeature
{
    /// <summary>
    /// Feature principale pour les slots étendus.
    /// Toute la logique est portée par les patches Harmony,
    /// donc pour l'instant Enable/Disable restent vides.
    /// </summary>
    public class ExtraSlotsFeature : IFeature
    {
        public void Enable()
        {
            // Les patches Harmony définis dans ce namespace seront appliqués
            // automatiquement par PatchManager via l'assembly du mod.
        }

        public void Disable()
        {
            // Plus tard, si tu veux un unpatch ciblé, tu pourras stocker ici les
            // références nécessaires (mais pour l'instant on s'appuie sur UnpatchSelf).
        }
    }
}

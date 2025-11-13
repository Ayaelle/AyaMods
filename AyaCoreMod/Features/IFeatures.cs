namespace AyaCoreMod.Features
{
    /// <summary>
    /// Contrat minimal d'une "feature" activable/désactivable.
    /// Chaque grande fonctionnalité d'un mod peut être encapsulée dans une IFeature.
    /// </summary>
    public interface IFeature
    {
        void Enable();
        void Disable();
    }
}

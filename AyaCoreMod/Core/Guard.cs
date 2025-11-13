using UnityEngine;

namespace AyaCoreMod.Core
{
    /// <summary>
    /// Helpers pour savoir si l'UI est occupée (PDA, menu, etc.).
    /// Utile pour éviter de déclencher des actions pendant qu'une interface est ouverte.
    /// </summary>
    public static class Guard
    {
        public static bool UIBusy()
        {
            var player = Player.main;
            if (player != null)
            {
                var pda = player.GetPDA();
                if (pda != null && pda.isOpen)
                    return true;
            }

            if (IngameMenu.main != null && IngameMenu.main.isActiveAndEnabled)
                return true;

            // Tu pourras ajouter ici un flag pour la console si tu patches DevConsole.SetState

            return false;
        }
    }
}

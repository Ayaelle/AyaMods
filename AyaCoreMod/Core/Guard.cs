using UnityEngine;

namespace AyaCoreMod.Core
{
    public static class Guard
    {
        public static bool UIBusy()
        {
            var pda = PlayerPrefs.main ? PlayerPrefs.main.GetPDA() : null;
            if (pda && pda.isOpen) return true;
            if (IngameMenu.main && IngameMenu.main.isActiveAndEnabled) return true;
            return false;
        }
    }
}

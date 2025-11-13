using System.Collections;
using UnityEngine;

namespace AyaCoreMod.Core
{
    /// <summary>
    /// Gère l'initialisation tardive de GameInput.
    /// On évite d'appeler GameInput pendant Awake() pour prévenir les NullReference.
    /// </summary>
    public static class InputManager
    {
        /// <summary>
        /// Indique si GameInput est prêt à être utilisé sans erreur.
        /// </summary>
        public static bool Ready { get; private set; }

        /// <summary>
        /// Coroutine à lancer après le chargement d'une scène (via StartCoroutine).
        /// Essaie périodiquement d'accéder à GameInput jusqu'à ce que ce soit stable.
        /// </summary>
        public static IEnumerator DelayedInit()
        {
            for (int i = 0; i < 300; i++)
            {
                try
                {
                    GameInput.GetBinding(GameInput.Device.Keyboard, GameInput.Button.Slot1, GameInput.BindingSet.Primary);
                    Ready = true;
                    break;
                }
                catch
                {
                    // GameInput pas prêt, on attend une frame
                    yield return null;
                }
            }

            Log.Info("[AyaCoreMod] InputManager.Ready = " + Ready);
        }
    }
}

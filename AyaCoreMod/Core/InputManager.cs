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
                bool ok = false;

                try
                {
                    // On teste un accès simple à GameInput
                    GameInput.GetBinding(GameInput.Device.Keyboard, GameInput.Button.Slot1, GameInput.BindingSet.Primary);
                    ok = true;
                }
                catch
                {
                    // GameInput pas encore prêt, on ne fait rien ici
                }

                if (ok)
                {
                    Ready = true;
                    break;
                }

                // On attend une frame avant de réessayer
                yield return null;
            }

            Log.Info("[AyaCoreMod] InputManager.Ready = " + Ready);
        }
    }
}

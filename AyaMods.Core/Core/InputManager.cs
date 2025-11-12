using Nautilus.Handlers;
using System.Collections;
using UnityEngine;

namespace AyaMods.Core
{
    public static class InputManager
    {
        public static bool Ready { get; private set; }

        public static IEnumerator DelayedInit()
        {
            for (int i = 0; i < 300; i++)
            {
                try
                {
                    GameInput.GetBinding(GameInputHandler.Device.Keyboard, GameInput.Button.Slot1, GameInputHandler.BindingSet.Primary);
                    Ready = true; break;
                }
                catch { yield return null; }
            }
            Log.Info("[AyaMods.Core] Input ready=" + Ready);
        }
    }
}

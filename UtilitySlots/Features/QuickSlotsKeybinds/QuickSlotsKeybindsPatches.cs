using AyaCoreMod.Core;
using HarmonyLib;
using UnityEngine;
using UtilitySlots.Config;

namespace UtilitySlots.Features.QuickSlotsKeybinds
{
    /// <summary>
    /// Remplace la gestion des quickslot buttons dans uGUI_QuickSlots.HandleInput
    /// pour utiliser nos keybinds QuickSlot1..12.
    /// </summary>
    [HarmonyPatch(typeof(uGUI_QuickSlots), "HandleInput")]
    public static class QuickSlotsKeybindsPatches
    {
        static bool Prefix(uGUI_QuickSlots __instance)
        {
            var type = typeof(uGUI_QuickSlots);
            var targetField = AccessTools.Field(type, "target");
            var target = targetField?.GetValue(__instance) as IQuickSlots;
            if (target == null)
                return false;

            if (!Player.main.GetCanItemBeUsed())
                return false;

            bool introOrCinematic = uGUI.isIntro || IntroLifepodDirector.IsActive;

            // On remplace la boucle sur quickSlotButtons (5 entrées) par nos 12 bind custom.
            if (!introOrCinematic)
            {
                var buttons = new[]
                {
                    Keybinds.QuickSlot1,
                    Keybinds.QuickSlot2,
                    Keybinds.QuickSlot3,
                    Keybinds.QuickSlot4,
                    Keybinds.QuickSlot5,
                    Keybinds.QuickSlot6,
                    Keybinds.QuickSlot7,
                    Keybinds.QuickSlot8,
                    Keybinds.QuickSlot9,
                    Keybinds.QuickSlot10,
                    Keybinds.QuickSlot11,
                    Keybinds.QuickSlot12
                };

                for (int i = 0; i < buttons.Length; i++)
                {
                    var action = buttons[i];
                    if (GameInput.GetButtonDown(action))
                    {
                        target.SlotKeyDown(i);
                    }
                    else if (GameInput.GetButtonHeld(action))
                    {
                        target.SlotKeyHeld(i);
                    }
                    if (GameInput.GetButtonUp(action))
                    {
                        target.SlotKeyUp(i);
                    }
                }

                // On garde les binds vanilla pour CycleNext/CyclePrev
                if (GameInput.GetButtonDown(GameInput.Button.CycleNext))
                {
                    target.SlotNext();
                }
                else if (GameInput.GetButtonDown(GameInput.Button.CyclePrev))
                {
                    target.SlotPrevious();
                }
            }

            if (AvatarInputHandler.main != null && AvatarInputHandler.main.IsEnabled())
            {
                if (GameInput.GetButtonDown(GameInput.Button.LeftHand))
                {
                    target.SlotLeftDown();
                }
                else if (GameInput.GetButtonHeld(GameInput.Button.LeftHand))
                {
                    target.SlotLeftHeld();
                }
                if (GameInput.GetButtonUp(GameInput.Button.LeftHand))
                {
                    target.SlotLeftUp();
                }
                if (GameInput.GetButtonDown(GameInput.Button.RightHand))
                {
                    target.SlotRightDown();
                }
                else if (GameInput.GetButtonHeld(GameInput.Button.RightHand))
                {
                    target.SlotRightHeld();
                }
                if (GameInput.GetButtonUp(GameInput.Button.RightHand))
                {
                    target.SlotRightUp();
                }
                if (!introOrCinematic && GameInput.GetButtonDown(GameInput.Button.Exit))
                {
                    target.DeselectSlots();
                }
            }

            // On a géré toute l'entrée nous-mêmes, on bloque l'original.
            return false;
        }
    }
}

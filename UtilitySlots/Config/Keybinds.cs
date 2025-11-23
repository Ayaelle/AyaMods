using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine.InputSystem;

namespace UtilitySlots.Config
{
    public static class Keybinds
    {
        public static global::GameInput.Button InternalUpgrades { get; private set; }
        public static global::GameInput.Button InternalStorage { get; private set; }

        public static void Register()
        {
            // Catégorie pour le menu "Mod Input"
            const string category = "UtilitySlots";

            // --- INTERNAL UPGRADES KEY ---
            InternalUpgrades =
                EnumHandler.AddEntry<global::GameInput.Button>("InternalUpgrades")
                    .CreateInput("Internal Upgrades Key")
                    .WithKeyboardBinding("<Keyboard>/u")
                    .WithCategory(category);

            // --- INTERNAL STORAGE KEY ---
            InternalStorage =
                EnumHandler.AddEntry<global::GameInput.Button>("InternalStorage")
                    .CreateInput("Internal Storage Key")
                    .WithKeyboardBinding("<Keyboard>/i")
                    .WithCategory(category);
        }
    }
}

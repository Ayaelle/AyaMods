using Nautilus.Handlers;

namespace UtilitySlots.Config
{
    public static class Keybinds
    {
        public static global::GameInput.Button InternalUpgrades { get; private set; }
        public static global::GameInput.Button InternalStorage { get; private set; }

        public static void Register()
        {

            // --- INTERNAL UPGRADES KEY ---
            InternalUpgrades =
                EnumHandler.AddEntry<global::GameInput.Button>("InternalUpgrades")
                    .CreateInput("Internal Upgrades Key")
                    .WithKeyboardBinding("<Keyboard>/u");

            // --- INTERNAL STORAGE KEY ---
            InternalStorage =
                EnumHandler.AddEntry<global::GameInput.Button>("InternalStorage")
                    .CreateInput("Internal Storage Key")
                    .WithKeyboardBinding("<Keyboard>/i");
        }
    }
}

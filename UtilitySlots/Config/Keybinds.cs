using Nautilus.Handlers;

namespace UtilitySlots.Config
{
    public static class Keybinds
    {
        //Internal Access
        public static global::GameInput.Button InternalUpgrades { get; private set; }
        public static global::GameInput.Button InternalStorage { get; private set; }

        //QuickSlots
        public static global::GameInput.Button QuickSlot1 { get; private set; }
        public static global::GameInput.Button QuickSlot2 { get; private set; }
        public static global::GameInput.Button QuickSlot3 { get; private set; }
        public static global::GameInput.Button QuickSlot4 { get; private set; }
        public static global::GameInput.Button QuickSlot5 { get; private set; }
        public static global::GameInput.Button QuickSlot6 { get; private set; }
        public static global::GameInput.Button QuickSlot7 { get; private set; }
        public static global::GameInput.Button QuickSlot8 { get; private set; }
        public static global::GameInput.Button QuickSlot9 { get; private set; }
        public static global::GameInput.Button QuickSlot10 { get; private set; }
        public static global::GameInput.Button QuickSlot11 { get; private set; }
        public static global::GameInput.Button QuickSlot12 { get; private set; }


        public static void Register()
        {

            // --- INTERNAL UPGRADES KEY ---
            InternalUpgrades = EnumHandler.AddEntry<global::GameInput.Button>("InternalUpgrades")
                .CreateInput("Internal Upgrades Key")
                .WithKeyboardBinding("<Keyboard>/u");

            // --- INTERNAL STORAGE KEY ---
            InternalStorage = EnumHandler.AddEntry<global::GameInput.Button>("InternalStorage")
                .CreateInput("Internal Storage Key")
                .WithKeyboardBinding("<Keyboard>/i");

            // --- QUICKSLOTS ---
            QuickSlot1 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot1")
                .CreateInput("QuickSlot 1")
                .WithKeyboardBinding("<Keyboard>/1");

            QuickSlot2 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot2")
                .CreateInput("QuickSlot 2")
                .WithKeyboardBinding("<Keyboard>/2");

            QuickSlot3 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot3")
                .CreateInput("QuickSlot 3")
                .WithKeyboardBinding("<Keyboard>/3");

            QuickSlot4 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot4")
                .CreateInput("QuickSlot 4")
                .WithKeyboardBinding("<Keyboard>/4");

            QuickSlot5 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot5")
                .CreateInput("QuickSlot 5")
                .WithKeyboardBinding("<Keyboard>/5");

            QuickSlot6 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot6")
                .CreateInput("QuickSlot 6")
                .WithKeyboardBinding("<Keyboard>/6");

            QuickSlot7 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot7")
                .CreateInput("QuickSlot 7")
                .WithKeyboardBinding("<Keyboard>/7");

            QuickSlot8 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot8")
                .CreateInput("QuickSlot 8")
                .WithKeyboardBinding("<Keyboard>/8");

            QuickSlot9 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot9")
                .CreateInput("QuickSlot 9")
                .WithKeyboardBinding("<Keyboard>/9");

            QuickSlot10 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot10")
                .CreateInput("QuickSlot 10")
                .WithKeyboardBinding("<Keyboard>/10");

            QuickSlot11 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot11")
                .CreateInput("QuickSlot 11")
                .WithKeyboardBinding("<Keyboard>/11");

            QuickSlot12 = EnumHandler.AddEntry<global::GameInput.Button>("QuickSlot12")
                .CreateInput("QuickSlot 12")
                .WithKeyboardBinding("<Keyboard>/12");
        }
    }
}

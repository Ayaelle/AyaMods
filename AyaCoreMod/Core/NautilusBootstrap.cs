using Nautilus.Handlers;
using Nautilus.Json;
using System.Net.NetworkInformation;

namespace AyaCoreMod.Core
{
    /// <summary>
    /// Facilite l'enregistrement automatique des options Nautilus.
    /// Utilisable par tous les mods AyaMods.
    /// </summary>
    public static class NautilusBootstrap
    {
        /// <summary>
        /// Enregistre le menu d'options Nautilus pour une classe de config.
        /// La classe T doit hériter de ConfigFile et avoir un ctor public sans paramètre.
        /// </summary>
        public static void Register<T>() where T : ConfigFile, new()
        {
            OptionsPanelHandler.RegisterModOptions<T>();
            // Pas de KeybindHandler dans Nautilus :
            // les [Keybind] sur ConfigFile sont gérés par le système de config lui-même.
        }
    }
}

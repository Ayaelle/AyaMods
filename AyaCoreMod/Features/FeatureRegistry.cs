using System;
using System.Collections.Generic;

namespace AyaCoreMod.Features
{
    /// <summary>
    /// Permet de gérer l'activation/désactivation de fonctionnalités (IFeature) par type.
    /// Evite de multiplier les singletons dans les mods.
    /// </summary>
    public static class FeatureRegistry
    {
        private static readonly Dictionary<Type, IFeature> _features = new Dictionary<Type, IFeature>();

        public static void Enable<T>() where T : IFeature, new()
        {
            var type = typeof(T);
            if (_features.ContainsKey(type))
                return;

            var feature = new T();
            feature.Enable();
            _features[type] = feature;
        }

        public static void Disable<T>() where T : IFeature
        {
            var type = typeof(T);
            if (_features.TryGetValue(type, out var feature))
            {
                feature.Disable();
                _features.Remove(type);
            }
        }

        public static void DisableAll()
        {
            foreach (var kvp in _features)
                kvp.Value.Disable();

            _features.Clear();
        }
    }
}

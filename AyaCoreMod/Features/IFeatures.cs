using System;
using System.Collections.Generic;

namespace AyaCoreMod.Feature
{
    public interface IFeature
    {
        void Enable();
        void Disable();
    }

    public static class FeatureRegistry
    {
        static readonly Dictionary<Type, IFeature> _map = new();

        public static void Enable<T>() where T : IFeature, new()
        {
            var t = typeof(T);
            if (_map.ContainsKey(t)) return;
            var f = new T();
            f.Enable();
            _map[t] = f;
        }

        public static void Disable<T>() where T : IFeature
        {
            var t = typeof(T);
            if (_map.TryGetValue(t, out var f)) { f.Disable(); _map.Remove(t); }
        }

        public static void DisableAll()
        {
            foreach (var it in _map) it.Value.Disable();
            _map.Clear();
        }
    }
}

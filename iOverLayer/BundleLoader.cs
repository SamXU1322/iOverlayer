using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;


namespace iOverlayer
{
    public static class BundleLoader
    {
        private static readonly Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();
        private static readonly string _bundlePath = Path.Combine(MelonEnvironment.UserDataDirectory, "iOverlayer", "Asset");

        private static AssetBundle GetBundle(string bundleName)
        {
            if (_bundles.TryGetValue(bundleName, out var cached))
                return cached;

            var path = Path.Combine(_bundlePath, bundleName);
            if (!File.Exists(path))
            {
                MelonLogger.Error($"AssetBundle not found: {path}");
                return null;
            }

            var bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                MelonLogger.Error($"Failed to load AssetBundle: {path}");
                return null;
            }

            _bundles[bundleName] = bundle;
            return bundle;
        }

        public static GameObject LoadPrefab(string bundleName, string prefabName)
        {
            var bundle = GetBundle(bundleName);
            if (bundle == null)
                return null;

            var prefab = bundle.LoadAsset<GameObject>(prefabName);
            if (prefab == null)
            {
                MelonLogger.Error($"Prefab '{prefabName}' not found in bundle '{bundleName}'");
                return null;
            }

            return prefab;
        }

        public static GameObject Instantiate(string bundleName, string prefabName, Transform parent = null)
        {
            var prefab = LoadPrefab(bundleName, prefabName);
            if (prefab == null)
                return null;

            return Object.Instantiate(prefab, parent);
        }

        public static void UnloadBundle(string bundleName)
        {
            if (_bundles.TryGetValue(bundleName, out var bundle))
            {
                bundle.Unload(false);
                _bundles.Remove(bundleName);
            }
        }

        public static void UnloadAll()
        {
            foreach (var bundle in _bundles.Values)
                bundle.Unload(false);
            _bundles.Clear();
        }
    }
}

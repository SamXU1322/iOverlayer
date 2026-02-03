using System.IO;
using TMPro;
using UnityEngine;
using UnityModManagerNet;
namespace iOverLayer
{
    public static class AssetLoader
    {
        private static string _assetBundlePrfabPath = "iOverLayer.assets/prefabs";
        private static string _assetBundleFontPath = "iOverLayer.assets/fonts";
        public static void Init(UnityModManager.ModEntry modEntry)
        {
            _assetBundlePrfabPath = Path.Combine(modEntry.Path, _assetBundlePrfabPath);
        }
        public static GameObject LoadPrefabAssetBundle(string AssetBundleName, string PrefabName)
        {
            if (!File.Exists(Path.Combine(_assetBundlePrfabPath, AssetBundleName)))
            {
                LogSystem.Error($"AssetBundle {AssetBundleName} not found at {_assetBundlePrfabPath}");
                return null;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(_assetBundlePrfabPath, AssetBundleName));
            if (!assetBundle)
            {
                LogSystem.Error($"Failed to load AssetBundle {AssetBundleName}");
                return null;
            }
            GameObject prefab = assetBundle.LoadAsset<GameObject>(PrefabName);
            assetBundle.Unload(false);
            return prefab;
        }
        public static TMP_FontAsset LoadFontAssetBundle(string AssetBundleName, string FontName)
        {
            if (!File.Exists(Path.Combine(_assetBundleFontPath, AssetBundleName)))
            {
                LogSystem.Error($"AssetBundle {AssetBundleName} not found at {_assetBundleFontPath}");
                return null;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(_assetBundleFontPath, AssetBundleName));
            if (!assetBundle)
            {
                LogSystem.Error($"Failed to load AssetBundle {AssetBundleName}");
                return null;
            }
            TMP_FontAsset FontAsset = assetBundle.LoadAsset<TMP_FontAsset>(FontName);
            assetBundle.Unload(false);
            return FontAsset;
        }
    }
}

using System.IO;
using UnityEngine;
using UnityModManagerNet;
namespace iOverLayer
{
    public static class AssetLoader
    {
        private static string _assetBundlePath = "iOverLayerAssets";
        public static void Init(UnityModManager.ModEntry modEntry)
        {
            _assetBundlePath = Path.Combine(modEntry.Path, _assetBundlePath);
        }
        public static GameObject LoadAssetBundle(string AssetBundleName, string PrefabName)
        {
            if (!File.Exists(Path.Combine(_assetBundlePath, AssetBundleName)))
            {
                LogSystem.Error($"AssetBundle {AssetBundleName} not found at {_assetBundlePath}");
                return null;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(_assetBundlePath, AssetBundleName));
            if (!assetBundle)
            {
                LogSystem.Error($"Failed to load AssetBundle {AssetBundleName}");
                return null;
            }
            GameObject prefab = assetBundle.LoadAsset<GameObject>(PrefabName);
            assetBundle.Unload(false);
            return prefab;
        }
    }
}

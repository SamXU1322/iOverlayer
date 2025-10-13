using System.Reflection;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using iOverlayer.Core;
using iOverlayer.Text;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace iOverlayer
{
    public static class Main 
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        public static Harmony harmony;
        private static GameObject _modGameObject;
        private static AssetBundle _assetBundle;
        private static GameObject iOverlagerText;

        public static void Start(UnityModManager.ModEntry modEntry)
        {
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            Start(modEntry);
            ModEntry = modEntry;
            _modGameObject = new GameObject("iOverlayer");
            Object.DontDestroyOnLoad(_modGameObject);
            InitializedPublicCanvas();
            string fontPath = "C:\\Users\\ASUS\\AppData\\Local\\Microsoft\\Windows\\Fonts\\Maplestory-Bold.otf";
            Logger.Log("UWU");
            LoadAssetBundle(modEntry);
            if (iOverlagerText != null)
            {
                iOverlagerText.transform.SetParent(_modGameObject.transform);
                Object.DontDestroyOnLoad(iOverlagerText);
            }
            
        }

        public static void InitializedPublicCanvas()
        {
            Canvas mainCanvas = _modGameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = _modGameObject.AddComponent<CanvasScaler>(); 
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; 
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = 0.5f;
        }

        public static void LoadAssetBundle(UnityModManager.ModEntry modEntry)
        {
            string AssetBundlePath = System.IO.Path.Combine(modEntry.Path, "text");
            if (System.IO.File.Exists(AssetBundlePath))
            {
                _assetBundle = AssetBundle.LoadFromFile(AssetBundlePath);
                GameObject TextPrefab = _assetBundle.LoadAsset<GameObject>("text");
                _assetBundle.Unload(false);
                if (TextPrefab != null)
                {
                    iOverlagerText = Object.Instantiate(TextPrefab);
                }
            }
        }
    }
}

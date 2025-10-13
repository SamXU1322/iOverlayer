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
            GameObject textObject = CreateTextObject();
            TextBehavior textBehavior = textObject.AddComponent<TextBehavior>();
            textBehavior.Initialize();
        }

        public static void InitializedPublicCanvas()
        {
            Canvas mainCanvas = _modGameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = _modGameObject.AddComponent<CanvasScaler>(); 
            scaler.referenceResolution = new Vector2(3840, 2160);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; 
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = 0.5f;
            _modGameObject.AddComponent<GraphicRaycaster>();
        }
        private static GameObject CreateTextObject()
        {
            string assetBundlePath = System.IO.Path.Combine(ModEntry.Path, "text");
            AssetBundle ba = AssetBundle.LoadFromFile(assetBundlePath);
            if (ba != null)
            {
                GameObject textPrefab = ba.LoadAsset<GameObject>("text");
                ba.Unload(false);
                if (textPrefab != null)
                {
                    return GameObject.Instantiate(textPrefab, _modGameObject.transform);
                }
                else
                {
                    Logger.Log("Failed to load 'text' prefab from AssetBundle");
                }
            }
            else
            {
                Logger.Log("Failed to load AssetBundle from path: " + assetBundlePath);
            }
            return null;
        }
    }
}

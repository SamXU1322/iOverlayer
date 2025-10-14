using System.Reflection;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using iOverlayer.Core;
using iOverlayer.Text;
using iOverlayer.GUI;
using TMPro;
using UnityEngine.TextCore.LowLevel;
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
        public static GameObject GUIObject;
        public static TMP_FontAsset BoldFontAsset;
        public static TMP_FontAsset HeavyFontAsset;
        public static TMP_FontAsset RegularFontAsset;
        

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
            SetDefaultFont();
            string fontPath = "C:\\Users\\ASUS\\AppData\\Local\\Microsoft\\Windows\\Fonts\\Maplestory-Bold.otf";
            // GameObject textObject = CreateTextObject();
            // TextBehavior textBehavior = textObject.AddComponent<TextBehavior>();
            // textBehavior.Initialize();
            GUIObject = CreateUIObject();
            iOverlayerGUI iOverlayerGUI =  GUIObject.AddComponent<iOverlayerGUI>();
            iOverlayerGUI.Initialize();
            
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
        private static GameObject CreateUIObject()
        {
            string assetBundlePath = System.IO.Path.Combine(ModEntry.Path, "ioverlayergui");
            AssetBundle ba = AssetBundle.LoadFromFile(assetBundlePath);
            if (ba != null)
            {
                GameObject GUIPrefab = ba.LoadAsset<GameObject>("iOverlayerGUI");
                ba.Unload(false);
                if (GUIPrefab != null)
                {
                    return GameObject.Instantiate(GUIPrefab, _modGameObject.transform);
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
        private static void SetDefaultFont()
        {
            string BoldFontPath = System.IO.Path.Combine(ModEntry.Path, "Fonts", "PingFang Bold.ttf").Replace("\\", "\\\\");
            string RegularFontPath = System.IO.Path.Combine(ModEntry.Path, "Fonts", "PingFang Regular.ttf").Replace("\\", "\\\\");
            string HeavyFontPath = System.IO.Path.Combine(ModEntry.Path, "Fonts", "PingFang Heavy.ttf").Replace("\\", "\\\\");
            Logger.Log($"{BoldFontPath}");
            if (System.IO.File.Exists(BoldFontPath) && System.IO.File.Exists(RegularFontPath) && System.IO.File.Exists(HeavyFontPath))
            {
                Font BoldFont = new Font(BoldFontPath);
                Font RegularFont = new Font(RegularFontPath);
                Font HeavyFont = new Font(HeavyFontPath);
                BoldFontAsset = TMP_FontAsset.CreateFontAsset(BoldFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
                RegularFontAsset = TMP_FontAsset.CreateFontAsset(RegularFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
                HeavyFontAsset = TMP_FontAsset.CreateFontAsset(HeavyFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
                Logger.Log("Default fonts loaded successfully.");
            }
            else
            {
                Logger.Log("Failed to load default fonts from path: " + ModEntry.Path);
            }
        }
    }
}

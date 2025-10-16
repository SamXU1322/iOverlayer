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
        public static GameObject ModGameObject;
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
            ModGameObject = new GameObject("iOverlayer");
            ModGameObject.AddComponent<ModManager>();
            Object.DontDestroyOnLoad(ModGameObject);
            SetDefaultFont();
            // GameObject textObject = CreateTextObject();
            // TextBehavior textBehavior = textObject.AddComponent<TextBehavior>();
            // textBehavior.Initialize();
            
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
                    return GameObject.Instantiate(textPrefab, ModGameObject.transform);
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

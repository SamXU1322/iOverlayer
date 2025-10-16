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
    }
}

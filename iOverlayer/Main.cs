using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using Object = UnityEngine.Object;
using iOverlayer.Core;
using TMPro;

namespace iOverlayer
{
    public static class Main 
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        internal static TextBehavior TextGUI;
        public static Harmony Harmony;
        public static KeyManager keyManager;
        
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            FontManager.Initialize();
            TextGUI = new GameObject().AddComponent<TextBehavior>();
            TextGUI.SetText("Congratulation");
            TextGUI.SetPosition(0.5f,0.5f);
            TextGUI.SetSize(222);
            Object.DontDestroyOnLoad(TextGUI);
            TextGUI.textObject.SetActive(true);
            string fontPath = "C:/Windows/Fonts/Maplestory-Bold.otf";
            if (System.IO.File.Exists(fontPath))
            {
                modEntry.Logger.Log("Font found");
            }
            else
            {
                modEntry.Logger.Log("Font not found");
            }
            modEntry.Logger.Log("UWU");
            keyManager = new GameObject("KeyManager").AddComponent<KeyManager>();
            keyManager.Initialize(modEntry);
            Object.DontDestroyOnLoad(keyManager.gameObject);
        }
    }
}

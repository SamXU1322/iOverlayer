using System.Reflection;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using Object = UnityEngine.Object;
using iOverlayer.Core;
using TMPro;
using UnityEngine.TextCore.LowLevel;

namespace iOverlayer
{
    public static class Main 
    {
        public static bool IsEnabled { get; private set; }
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        internal static TextBehavior TextGUI;
        public static Harmony harmony;
        public static KeyManager keyManager;

        public static void Start(UnityModManager.ModEntry modEntry)
        {
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        
        
        
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            Start(modEntry);
            FontManager.Initialize();
            TextGUI = new GameObject().AddComponent<TextBehavior>();
            TextGUI.SetText("Congratulation");
            TextGUI.SetPosition(0.5f,0.5f);
            TextGUI.SetSize(222);
            TextGUI.textObject.SetActive(true);
            Object.DontDestroyOnLoad(TextGUI);
            string fontPath = "C:\\Users\\ASUS\\AppData\\Local\\Microsoft\\Windows\\Fonts\\Maplestory-Bold.otf";
    
            modEntry.Logger.Log("UWU");
            keyManager = new GameObject("KeyManager").AddComponent<KeyManager>();
            keyManager.Initialize(modEntry);
            Object.DontDestroyOnLoad(keyManager.gameObject);
        }
    }
}

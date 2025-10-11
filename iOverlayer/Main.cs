using System;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using iOverlayer.Core;
using iOverlayer.Text;
using Object = UnityEngine.Object;

namespace iOverlayer
{
    public static class Main 
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        internal static TextBehavior TextGUI;
        public static Harmony harmony;
        private static iOverlayerUI _overlayerUI;
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
            TextGUI = new GameObject().AddComponent<TextBehavior>();
            TextGUI.SetText("Congratulation");
            TextGUI.SetPosition(0.5f,0.5f);
            TextGUI.SetSize(128);
            TextGUI.textObject.SetActive(true);
            Object.DontDestroyOnLoad(TextGUI);
            string fontPath = "C:\\Users\\ASUS\\AppData\\Local\\Microsoft\\Windows\\Fonts\\Maplestory-Bold.otf";
            TextGUI.SetFont(fontPath);
            Logger.Log("UWU");
            _modGameObject = new GameObject("iOverlayerUI");
            _overlayerUI = _modGameObject.AddComponent<iOverlayerUI>();
            _overlayerUI.Initilize(Logger);
            Object.DontDestroyOnLoad(_modGameObject);
        }
    }
}

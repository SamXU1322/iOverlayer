using System.IO;
using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
using UnityEngine;
using TMPro;
namespace iOverlayer
{
    public static class Main 
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        internal static TextBehavior TextGUI;
        public static Harmony Harmony;
        
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            TextGUI = new GameObject().AddComponent<TextBehavior>();
            TextGUI.SetText("Congratulations!");
            TextGUI.SetPosition(0.5f,0.5f);
            TextGUI.SetSize(222);
            UnityEngine.Object.DontDestroyOnLoad(TextGUI);
            TextGUI.textObject.SetActive(true);
            Logger.Log("uwu");
        }
    }
}

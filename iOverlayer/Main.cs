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
        internal static TextBehavior gui;
        public static Harmony Harmony;
        
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            gui = new GameObject().AddComponent<TextBehavior>();
            gui.setText("Congratulations!");
            gui.setPosition(0.5f,0.5f);
            gui.setSize(222);
            UnityEngine.Object.DontDestroyOnLoad(gui);
            gui.TextObject.SetActive(true);
            modEntry.Logger.Log("uwu");
        }
    }
}

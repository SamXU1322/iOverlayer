using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using HarmonyLib;
using UnityModManagerNet;
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
            gui.setText("uwu");
            gui.setPosition(0.5f,0.5f);
            UnityEngine.Object.DontDestroyOnLoad(gui);
            gui.TextObject.SetActive(true);
            modEntry.Logger.Log("Hello World");
        }
    }
}

using HarmonyLib;
using iOverLayer.Text;
using UnityEngine;
using UnityModManagerNet;
namespace iOverLayer{
    public static class Main
    {
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            LogSystem.Init(modEntry);
            AssetLoader.Init(modEntry);
            Canvas.Init();
            TextManager.Create();
        }
    }
}
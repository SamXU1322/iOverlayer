using UnityEngine;
using HarmonyLib;
using UnityModManagerNet;
namespace iOverlayer
{
    public static class Main 
    {
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static UnityModManager.ModEntry ModEntry;
        internal static TextBehavior TextGUI;
        public static Harmony Harmony;
        private static KeyManager keyManager;
        
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            TextGUI = new GameObject().AddComponent<TextBehavior>();
            TextGUI.SetText("Congratulations!");
            TextGUI.SetPosition(0.5f,0.5f);
            TextGUI.SetSize(222);
            Object.DontDestroyOnLoad(TextGUI);
            TextGUI.textObject.SetActive(true);

            keyManager = new GameObject("KeyManager").AddComponent<KeyManager>();
            keyManager.Initialtize(modEntry);
            Object.DontDestroyOnLoad(keyManager.gameObject);
        }
    }
}

using iOverLayer.Patch;
using iOverLayer.UI;
using UnityModManagerNet;
namespace iOverLayer{
    public static class Main
    {
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            LogSystem.Init(modEntry);
            AssetLoader.Init(modEntry);
            Canvas.Init();
            UIManager.LoadMainUI();
            PatchManager.ApplyPatches("iOverLayer");
        }
    }
}
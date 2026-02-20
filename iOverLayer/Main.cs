using iOverLayer.Patch;
using iOverLayer.Patch.Patches;
using iOverLayer.Text;
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
            TextManager.Create("ComBo", "");
            
        }
    }
}
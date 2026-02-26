using iOverLayer.ADOFAI;
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
            AccPatch.AddPatch();
            TextManager.Create("Acc","");
            
            
        }
        [iOverLayerPatch(typeof(scnGame), "Play", PatchType.Postfix, false)]
        [iOverLayerPatch(typeof(scrPressToStart), "ShowText", PatchType.Postfix, false)]
        public static void OnGameStart()
        {
            
            Title.LevelInit();
            
        }

    }
}
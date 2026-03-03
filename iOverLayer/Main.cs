using iOverLayer.Setting;
using iOverLayer.Text;
using iOverLayer.UI;
using System.IO;
using UnityModManagerNet;
namespace iOverLayer
{
    public static class Main
    {
        public static UnityModManager.ModEntry mod;
        public static TestSetting setting;
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            LogSystem.Init(modEntry);
            AssetLoader.Init(modEntry);
            Canvas.Init();
            UIManager.LoadMainUI();
            TextManager.Init(Path.Combine(modEntry.Path,"1.json"));
        }
        
    }
}
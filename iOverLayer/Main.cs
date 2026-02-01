using UnityModManagerNet;
using iOverLayer;
namespace iOverLayer{
    public static class Main
    {
        public static void Load(UnityModManager.ModEntry modEntry)
        {
            LogSystem.Init(modEntry);
            AssetLoader.Init(modEntry);
        }
    }
}
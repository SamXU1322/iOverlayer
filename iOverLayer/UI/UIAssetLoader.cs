using UnityEngine;
namespace iOverLayer.UI
{
    public static class UIAssetLoader
    {
        private static GameObject  _mainUI = AssetLoader.LoadPrefabAssetBundle("gameobject","GameObject");
        public static void LoadMainUI()
        {
            if(_mainUI == null)
            {
                LogSystem.Error("MainUI Load error");
                return;
            }
            GameObject instance = Object.Instantiate(_mainUI);
            Object.DontDestroyOnLoad(instance);
            if (instance == null)
            {
                LogSystem.Error("Main UI prefab load failed.");
                return;
            }
        }
    }
}

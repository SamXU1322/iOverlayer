using Rewired.UI.ControlMapper;
using UnityEngine;
namespace iOverLayer.UI
{
    public static class UIManager
    {
        private static GameObject  _mainUI = AssetLoader.LoadPrefabAssetBundle("mainui","MainUI");
        public static GameObject MainUI => _mainUI;
        public static void LoadMainUI()
        {
            if(_mainUI == null)
            {
                LogSystem.Error("MainUI Load error");
                return;
            }
            GameObject instance = Object.Instantiate(_mainUI);
            
            if (instance == null)
            {
                LogSystem.Error("Main UI prefab load failed.");
                return;
            }
            else
            {
                Object.DontDestroyOnLoad(instance);
                instance.AddComponent<UIController>();
            } 
        }
    }
}

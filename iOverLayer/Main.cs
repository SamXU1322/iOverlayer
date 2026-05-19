using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using iOverlayer.UI;


[assembly: MelonInfo(typeof(iOverlayer.Core), iOverlayer.Info.Name, iOverlayer.Info.Version, iOverlayer.Info.Author, iOverlayer.Info.DownloadUrl)]
[assembly: MelonGame("7th Beat Games", "A Dance of Fire and Ice")]

namespace iOverlayer
{
    public class Core : MelonMod
    {
        private GameObject rootGo;
        private GameObject mainUI;
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("iOverlayer loaded");
            InitializeCanvas();
            
        }
        private void InitializeCanvas()
        {
            rootGo = new GameObject("iOverlayer_UI_Root");
            Object.DontDestroyOnLoad(rootGo);
            var canvas = rootGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            rootGo.AddComponent<CanvasScaler>();
            rootGo.AddComponent<GraphicRaycaster>();
            mainUI = BundleLoader.Instantiate("mainui", "MainUI");
            if (mainUI != null)
            {
                mainUI.transform.SetParent(rootGo.transform, false);
                mainUI.AddComponent<MainUI>();
                mainUI.SetActive(false);
            }
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (mainUI != null)
                {
                    mainUI.SetActive(!mainUI.activeSelf);
                }
            }
        }
    }
}

using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(iOverlayer.Core), iOverlayer.Info.Name, iOverlayer.Info.Version, iOverlayer.Info.Author, iOverlayer.Info.DownloadUrl)]
[assembly: MelonGame("7th Beat Games", "A Dance of Fire and Ice")]

namespace iOverlayer
{
    public class Core : MelonMod
    {
        private Rect windowRect = new Rect(20, 20, 240, 80);

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("iOverlayer loaded");
        }

        public override void OnGUI()
        {
            windowRect = GUI.Window(0, windowRect, DrawWindow, "iOverlayer");
        }

        private void DrawWindow(int windowID)
        {
            GUI.Label(new Rect(10, 20, 220, 40), "hello iOverlayer");
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}

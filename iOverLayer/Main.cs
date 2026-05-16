using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(iOverlayer.Core), iOverlayer.Info.Name, iOverlayer.Info.Version, iOverlayer.Info.Author, iOverlayer.Info.DownloadUrl)]
[assembly: MelonGame("7th Beat Games", "A Dance of Fire and Ice")]

namespace iOverlayer
{
    public class Core : MelonMod
    {
        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("iOverlayer loaded");
            UIModule.Initialize();

            string fontPath = @"E:\steam\steamapps\common\A Dance of Fire and Ice\Mods\Overlayer\Pretendard-Bold.otf";

            iOverlayText text = UIModule.CreateText("hello iOverlayer");
            text.SetFont(fontPath);
        }
    }
}

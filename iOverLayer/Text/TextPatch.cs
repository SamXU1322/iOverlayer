using iOverLayer.ADOFAI;
using iOverLayer.Patch;
using System.Reflection;

namespace iOverLayer.Text
{
    internal static class TextPatch
    {
        public static void AddPatch()
        {
            PatchManager.AddPatch(typeof(scnGame), "Play", typeof(TextPatch).GetMethod("OnGameStart",BindingFlags.Public|BindingFlags.Static),PatchType.Postfix, false);
            PatchManager.AddPatch(typeof(scrPressToStart), "ShowText", typeof(TextPatch).GetMethod("OnGameStart", BindingFlags.Public | BindingFlags.Static), PatchType.Postfix, false);
            PatchManager.AddPatch(typeof(scrUIController), "WipeToBlack", typeof(TextPatch).GetMethod("OnGameStop", BindingFlags.NonPublic | BindingFlags.Static), PatchType.Postfix, false);
            PatchManager.AddPatch(typeof(scnEditor), "ResetScene", typeof(TextPatch).GetMethod("OnGameStop", BindingFlags.NonPublic | BindingFlags.Static), PatchType.Postfix, false);
            PatchManager.AddPatch(typeof(scrController), "StartLoadingScene", typeof(TextPatch).GetMethod("OnGameStop", BindingFlags.NonPublic | BindingFlags.Static), PatchType.Postfix, false);

        }
        [iOverLayerPatch(typeof(scnGame), "Play", PatchType.Postfix, false)]
        [iOverLayerPatch(typeof(scrPressToStart), "ShowText", PatchType.Postfix, false)]
        public static void OnGameStart()
        {
            foreach (var text in TextManager.Texts)
            {
                text.Value.Show();
            }
            Title.LevelInit();
        }
        [iOverLayerPatch(typeof(scrUIController), "WipeToBlack", PatchType.Postfix, false)]
        [iOverLayerPatch(typeof(scnEditor), "ResetScene", PatchType.Postfix, false)]
        [iOverLayerPatch(typeof(scrController), "StartLoadingScene", PatchType.Postfix, false)]
        private static void OnGameStop()
        {
            foreach (var text in TextManager.Texts)
            {
                text.Value.UnShow();
            }
        }
    }
}

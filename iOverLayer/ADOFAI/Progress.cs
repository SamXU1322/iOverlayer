using iOverLayer.Text;
using System;

namespace iOverLayer.ADOFAI
{
    internal static class Progress
    {
        private static float _progressValue;
        public static float ProgressValue => _progressValue;
        public static void UpdateProgress()
        {
            _progressValue = scrController.instance.percentComplete;
            if(TextManager.Texts.ContainsKey("Progress"))
            {
#if DEBUG
                TextManager.Texts["Progress"].SetText("Progress: " + Math.Round(_progressValue * 100, 2).ToString() + "%");
#endif
            }
            else
            {
                LogSystem.Warning("Progress text not found.");
            }
        }
    }
}

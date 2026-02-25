using iOverLayer.Text;
using System;

namespace iOverLayer.ADOFAI
{
    internal static class Progress
    {
        private static float _progressValue;
        private static bool _isPrinted=false;
        public static float ProgressValue => _progressValue;
        public static void UpdateProgress()
        {
            _progressValue = scrController.instance.percentComplete;
            if (TextManager.Texts.ContainsKey("Progress"))
            {
                TextManager.Texts["Progress"].SetText("Progress: " + Math.Round(_progressValue * 100, 2).ToString() + "%");
            }
            else
            {
                if (_isPrinted) return;
                _isPrinted = true;
                LogSystem.Warning("Progress Tag is not existed.");
            }
                
            
            
        }
    }
}

using iOverLayer.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iOverLayer.ADOFAI
{
    internal static class TimingScale
    {
        private static bool _isPrinted = false;
        public static void UpdateTimingScale()
        {

            if (TextManager.Texts.ContainsKey("TimingScale"))
            {
                TextManager.Texts["TimingScale"].SetText($"Timing Scale - {Math.Round(scrController.instance.currFloor.marginScale * 100, 2)}%");
            }
            else
            {
                if (_isPrinted) return;
                _isPrinted = true;
                LogSystem.Warning("TimingScale text not found.");
            } 
        }
    }
}

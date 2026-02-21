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
        public static void UpdateTimingScale()
        {
            if (TextManager.Texts.ContainsKey("TimingScale"))
            {
#if DEBUG
                TextManager.Texts["TimingScale"].SetText($"Timing Scale - {Math.Round(scrController.instance.currFloor.marginScale * 100, 2)}%");
#endif
            }
            else
            {
                LogSystem.Warning("Progress text not found.");
            }
        }
    }
}

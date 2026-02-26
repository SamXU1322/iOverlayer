using iOverLayer.Text;
using System;
using UnityEngine.UIElements;

namespace iOverLayer.ADOFAI
{
    internal static class Accuracy
    {
        public static float acc;
        public static float xacc;
        public static float maxAcc;
        public static void UpdateAccuracy()
        {
            xacc = scrController.instance.mistakesManager?.percentXAcc ?? 1;
            xacc.SetIfNaN(1);
            acc = scrController.instance.mistakesManager?.percentAcc ?? 1;
            maxAcc = 1 + (scrController.instance.currentSeqID + 1) * 0.0001f;
            if (TextManager.Texts.ContainsKey("Acc"))
            {
                TextManager.Texts["Acc"].SetText($"<color=white>Accuracy |</color> {Math.Round(acc * 100, 2)}%");
            }
            if (TextManager.Texts.ContainsKey("XAcc"))
            {
                TextManager.Texts["XAcc"].SetText($"<color=white>XAccuracy |</color> {Math.Round(xacc * 100, 2)}%");
            }
        }
    }
}

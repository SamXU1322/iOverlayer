using iOverLayer.Text;
using System.Diagnostics;

namespace iOverLayer.ADOFAI
{
    internal static class ComBo
    {
        public static int Combo;
        private static bool _isPrinted = false;
        public static void UpdateComBo(int combo)
        {
            Combo = combo;
            if (TextManager.Texts.ContainsKey("ComBo"))
            {
                TextManager.Texts["ComBo"].SetText("Combo: " + Combo);
            }
            else
            {
                if (_isPrinted) return;
                _isPrinted = true;
                LogSystem.Warning("ComBo text is not existed.");
            }
        }
    }
}

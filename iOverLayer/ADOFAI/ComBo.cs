using iOverLayer.Text;
using System.Diagnostics;

namespace iOverLayer.ADOFAI
{
    internal static class ComBo
    {
        public static int Combo;
        public static void UpdateComBo(int combo)
        {
            Combo = combo;
            if (TextManager.Texts.ContainsKey("ComBo"))
            {
#if DEBUG
                TextManager.Texts["ComBo"].SetText("Combo: " + Combo);
#endif
            }
            else
            {
                LogSystem.Warning("Tile text not found.");
            }
        }
    }
}

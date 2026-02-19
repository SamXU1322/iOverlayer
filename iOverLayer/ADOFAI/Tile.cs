using iOverLayer.Text;
using System;

namespace iOverLayer.ADOFAI
{
    internal static class Tile
    {
        public static int LeftTile;
        public static int CurTile;
        public static int TotalTile;
        public static void UpdateTile()
        {
            CurTile = scrController.instance.currentSeqID + 1;
            TotalTile = ADOBase.lm.listFloors.Count;
            LeftTile = TotalTile - CurTile;
            if (TextManager.Texts.ContainsKey("Tile"))
            {
#if DEBUG
                TextManager.Texts["Tile"].SetText($"CurTile:{CurTile},TotalTile:{TotalTile},LeftTile:{LeftTile}");
#endif
            }
            else
            {
                LogSystem.Warning("Tile text not found.");
            }
        }
    }
}
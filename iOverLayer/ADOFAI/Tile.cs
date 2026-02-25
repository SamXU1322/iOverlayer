using iOverLayer.Text;
using System;
using System.Runtime.InteropServices.ComTypes;

namespace iOverLayer.ADOFAI
{
    internal static class Tile
    {
        private static bool _isPrinted = false;
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
                TextManager.Texts["Tile"].SetText($"CurTile:{CurTile},TotalTile:{TotalTile},LeftTile:{LeftTile}");
            }
            else
            {
                if (_isPrinted) return;
                _isPrinted = true;
                LogSystem.Warning("The Tile Tag is not existed");
            }
        }
    }
}
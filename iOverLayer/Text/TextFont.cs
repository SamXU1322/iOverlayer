using System.IO;
using TMPro;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextFont
    {
        private static string _fontPath;
        private static TMP_FontAsset _fontAsset;

        public static void SetFontPath(string fontPath)
        {
            _fontPath = fontPath;
            _fontAsset = null;
        }
    }
}

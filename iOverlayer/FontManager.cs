using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace iOverlayer
{
    public class FontManager
    {
        static TMP_FontAsset DefaultTMP;
        static Font DefaultFont;
        static bool initalized;
        static FontData defaultFont;
        static Dictionary<string, FontData> Fonts;
        public static bool Initialize() => initalized;
        public static string[] OSFonts { get; private set; }
        public static string[] OSFontsPath { get; private set; }

    }
}
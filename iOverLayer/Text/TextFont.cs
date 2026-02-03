using System.IO;
using TMPro;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextFont
    {
        private static TMP_FontAsset _fontAsset;
        public static void LoadFontAsset(string fontName) 
        {
            _fontAsset = AssetLoader.LoadFontAssetBundle("",fontName);
        }
    }
}

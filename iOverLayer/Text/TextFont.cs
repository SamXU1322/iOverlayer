using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextFont
    {
        private static Dictionary<string,TMP_FontAsset> _fontAsset = new Dictionary<string, TMP_FontAsset>();
        public static Dictionary<string, TMP_FontAsset> FontAsset => _fontAsset;
        public static void LoadFontAsset(string fontName) 
        {
            TMP_FontAsset tmpFontAsset = AssetLoader.LoadFontAssetBundle("font", fontName);
            if (tmpFontAsset != null)
            {
                _fontAsset[fontName] = tmpFontAsset;
                LogSystem.Info($"Load font asset '{fontName}' from asset bundle.");
            }
            else
            {
                LogSystem.Error($"{fontName} load error");
            }
        }
        public static void CreateOSAsset(string fontName)
        {
            Font osFont = Font.CreateDynamicFontFromOSFont(fontName, TextDefaultSetting.DefaultFontSize);
            if(osFont != null)
            {
                TMP_FontAsset tmpFontAsset = TMP_FontAsset.CreateFontAsset(osFont);
                _fontAsset[fontName] = tmpFontAsset;
                LogSystem.Info($"Create font asset '{fontName}' from OS font.");
            }
            else
            {
                LogSystem.Error($"{fontName} create error");
            }
        }
    }
}

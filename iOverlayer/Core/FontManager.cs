using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using TMPro;

namespace iOverlayer.Core
{
    public class FontManager
    {
        static TMP_FontAsset DefaultTMPFont;
        static Font DefaultFont;
        static bool initialized;
        static FontData defaultFont;
        static Dictionary<string, FontData> Fonts = new Dictionary<string, FontData>();
        public static bool Initialized => initialized;
        public static string[] OSFonts { get; private set; }
        public static string[] OSFontPaths { get; private set; }
        public static ReadOnlyCollection<FontData> FallbackFontDatas { get; private set; }
        public static ReadOnlyCollection<Font> FallbackFonts { get; private set; }
        public static ReadOnlyCollection<TMP_FontAsset> FallbackTMPFonts { get; private set; }
        public static FontData GetFontSafe(string name) => TryGetFont(name, out FontData font) ? font : defaultFont;
        public static void SetFont(string name, FontData font) => Fonts[name] = font;

        public static bool TryGetFont(string name, out FontData font)
        {
            if (string.IsNullOrEmpty(name))
            {
                font = defaultFont;
                return false;
            }

            if (name == "Default")
            {
                font = defaultFont;
                return true;
            }
            name = name.Replace("{ModDir}", Main.ModEntry.Path);
            if (Fonts.TryGetValue(name, out FontData data))
            {
                font = data;
                return true;
            }
            else
            {
                if (File.Exists(name))
                {
                    FontData newData = defaultFont;
                    Font newFont = new Font(name);
                    TMP_FontAsset newTMPFont = TMP_FontAsset.CreateFontAsset(newFont);
                    if (newTMPFont)
                    {
                        newTMPFont.fallbackFontAssetTable = FallbackTMPFonts.ToList();
                    }
                    newData.font = newFont;
                    newData.fontTMP = newTMPFont ?? defaultFont.fontTMP;
                    Fonts.Add(name, newData);
                    font = newData;
                    return true;
                }
            }
           font = defaultFont;
           return false;
        }
    }
}
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using iOverlayer.Editor;

namespace iOverlayer.Text
{
    public static class OverlayFonts
    {
        private static readonly Dictionary<string, TMP_FontAsset> _cache =
            new Dictionary<string, TMP_FontAsset>(StringComparer.OrdinalIgnoreCase);

        private static readonly Dictionary<string, Font> _fonts =
            new Dictionary<string, Font>(StringComparer.OrdinalIgnoreCase);

        public static TMP_FontAsset GetOrCreate(string familyName, string fontPath = null)
        {
            if (string.IsNullOrEmpty(familyName)) familyName = "Arial";

            var asset = TryCreate(familyName, fontPath);
            if (asset != null) return asset;

            if (!string.Equals(familyName, "Arial", StringComparison.OrdinalIgnoreCase))
                return TryCreate("Arial", null);
            return null;
        }

        private static TMP_FontAsset TryCreate(string familyName, string fontPath)
        {
            if (_cache.TryGetValue(familyName, out var cached))
                return cached;

            var resolvedPath = fontPath;
            if (string.IsNullOrEmpty(resolvedPath))
                resolvedPath = FindOsFontPath(familyName);

            if (string.IsNullOrEmpty(resolvedPath))
            {
                MelonLoader.MelonLogger.Error($"[OverlayFonts] No font file found for '{familyName}'");
                return null;
            }

            try
            {
                var font = new Font(resolvedPath);
                if (font == null)
                {
                    MelonLoader.MelonLogger.Error($"[OverlayFonts] Failed to create Font from '{resolvedPath}'");
                    return null;
                }

                var asset = TMP_FontAsset.CreateFontAsset(font);
                if (asset == null)
                {
                    MelonLoader.MelonLogger.Error($"[OverlayFonts] TMP_FontAsset.CreateFontAsset returned null for '{familyName}' ({resolvedPath})");
                    return null;
                }

                _fonts[familyName] = font;
                _cache[familyName] = asset;
                return asset;
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[iOverlayer] Failed to create TMP font '{familyName}': {ex.Message}");
                return null;
            }
        }

        private static string FindOsFontPath(string familyName)
        {
            try
            {
                var paths = Font.GetPathsToOSFonts();
                if (paths == null) return null;

                foreach (var path in paths)
                {
                    if (string.Equals(System.IO.Path.GetFileNameWithoutExtension(path), familyName,
                        StringComparison.OrdinalIgnoreCase))
                        return path;
                }

                foreach (var path in paths)
                {
                    try
                    {
                        var name = CustomFont.ReadFamilyName(path);
                        if (string.Equals(name, familyName, StringComparison.OrdinalIgnoreCase))
                            return path;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"[OverlayFonts] Failed to enumerate OS fonts: {ex.Message}");
            }
            return null;
        }
    }
}

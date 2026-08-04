using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using MelonLoader;
using UnityEngine;

namespace iOverlayer.Editor
{
    /// <summary>
    /// Registers a user font file (.ttf/.otf) with Windows at runtime so Unity can
    /// render it via Font.CreateDynamicFontFromOSFont by family name.
    /// </summary>
    public static class CustomFont
    {
        private static readonly Dictionary<string, string> _familyByPath =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Registers the font file (once per path) and returns its family name, or null on failure.</summary>
        public static string GetOrRegister(string fontPath)
        {
            if (string.IsNullOrWhiteSpace(fontPath)) return null;

            var trimmed = fontPath.Trim().Trim('"');
            if (trimmed.Length == 0) return null;

            string full;
            try { full = Path.GetFullPath(trimmed); }
            catch { full = trimmed; }

            if (_familyByPath.TryGetValue(full, out var cached))
                return cached;

            if (!File.Exists(full))
            {
                MelonLogger.Error($"[iOverlayer] Font file not found: {full}");
                return null;
            }

            var familyName = ReadFamilyName(full);
            if (string.IsNullOrEmpty(familyName))
            {
                MelonLogger.Error($"[iOverlayer] Could not read the family name from font file: {full}");
                return null;
            }

            try
            {
                if (AddFontResourceW(full) <= 0)
                {
                    MelonLogger.Error($"[iOverlayer] Failed to register font file (AddFontResource returned 0): {full}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[iOverlayer] Failed to register font file: {full} ({ex.Message})");
                return null;
            }

            _familyByPath[full] = familyName;
            MelonLogger.Msg($"[iOverlayer] Font registered: '{familyName}' <- {full}");
            return familyName;
        }

        /// <summary>Registers the font file and creates a dynamic Unity font from it.</summary>
        public static Font CreateDynamicFont(string fontPath, int size)
        {
            var familyName = GetOrRegister(fontPath);
            if (familyName == null) return null;

            try
            {
                var font = Font.CreateDynamicFontFromOSFont(familyName, Mathf.Max(1, size));
                if (font == null)
                    MelonLogger.Warning($"[iOverlayer] Unity could not create a font for '{familyName}'");
                return font;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[iOverlayer] Failed to create font '{familyName}': {ex.Message}");
                return null;
            }
        }

        // ---------- sfnt 'name' table parsing (.ttf/.otf) ----------

        public static string ReadFamilyName(string path)
        {
            var data = File.ReadAllBytes(path);
            if (data.Length < 12) return null;

            uint version = ReadU32(data, 0);
            bool isSfnt = version == 0x00010000 || version == 0x4F54544F /* OTTO */ ||
                          version == 0x74727565 /* true */ || version == 0x74797031 /* typ1 */;
            if (!isSfnt) return null;

            int numTables = ReadU16(data, 4);
            if (numTables <= 0 || numTables > 4096) return null;

            int nameOffset = -1;
            for (int i = 0; i < numTables; i++)
            {
                int rec = 12 + i * 16;
                if (rec + 16 > data.Length) break;
                if (Encoding.ASCII.GetString(data, rec, 4) == "name")
                {
                    nameOffset = (int)ReadU32(data, rec + 8);
                    break;
                }
            }
            if (nameOffset < 0 || nameOffset + 6 > data.Length) return null;

            int count = ReadU16(data, nameOffset + 2);
            int stringsBase = nameOffset + ReadU16(data, nameOffset + 4);

            string best = null;
            int bestScore = -1;
            for (int i = 0; i < count; i++)
            {
                int rec = nameOffset + 6 + i * 12;
                if (rec + 12 > data.Length) break;

                int platform = ReadU16(data, rec);
                int encoding = ReadU16(data, rec + 2);
                int language = ReadU16(data, rec + 4);
                int nameId = ReadU16(data, rec + 6);
                int len = ReadU16(data, rec + 8);
                int off = ReadU16(data, rec + 10);
                int abs = stringsBase + off;
                if (abs < 0 || abs + len > data.Length) continue;

                if (nameId != 1 && nameId != 16) continue; // family / typographic family

                string value = null;
                int score = 0;
                if (platform == 3 && (encoding == 1 || encoding == 10)) // Windows Unicode
                {
                    value = Encoding.BigEndianUnicode.GetString(data, abs, len);
                    score = 2 + (language == 0x0409 ? 1 : 0);
                }
                else if (platform == 0) // Unicode
                {
                    value = Encoding.BigEndianUnicode.GetString(data, abs, len);
                    score = 1 + (language == 0x0409 ? 1 : 0);
                }
                else if (platform == 1 && encoding == 0) // Mac Roman (approx. Latin-1)
                {
                    value = Encoding.GetEncoding(28591).GetString(data, abs, len);
                    score = 1;
                }
                if (value == null) continue;

                value = value.Trim().TrimEnd('\0').Trim();
                if (value.Length == 0) continue;

                int total = (nameId == 16 ? 64 : 0) + score; // prefer typographic family
                if (total > bestScore)
                {
                    bestScore = total;
                    best = value;
                }
            }
            return best;
        }

        private static ushort ReadU16(byte[] d, int i) => (ushort)((d[i] << 8) | d[i + 1]);
        private static uint ReadU32(byte[] d, int i) =>
            ((uint)d[i] << 24) | ((uint)d[i + 1] << 16) | ((uint)d[i + 2] << 8) | d[i + 3];

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int AddFontResourceW(string lpszFilename);
    }
}

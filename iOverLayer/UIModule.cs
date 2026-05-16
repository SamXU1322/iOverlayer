using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace iOverlayer
{
    public static class UIModule
    {
        public static GameObject CanvasObject { get; private set; }

        private static readonly List<iOverlayText> texts = new List<iOverlayText>();

        internal static void Initialize()
        {
            CanvasObject = new GameObject("iOverlayerCanvas");
            Canvas canvas = CanvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            CanvasObject.AddComponent<CanvasScaler>();
            GameObject.DontDestroyOnLoad(CanvasObject);
        }

        public static iOverlayText CreateText(string text = "", float x = 0, float y = 0, int fontSize = 48)
        {
            if (CanvasObject == null)
                throw new InvalidOperationException("UIModule not initialized");

            GameObject textObj = new GameObject("OverlayText");
            textObj.transform.SetParent(CanvasObject.transform, false);

            Text textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.fontSize = fontSize;
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleCenter;
            textComp.font = Font.CreateDynamicFontFromOSFont("Arial", 16);

            RectTransform rect = textObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(600, 80);

            iOverlayText overlay = new iOverlayText(textObj);
            texts.Add(overlay);
            return overlay;
        }

        public static Font LoadFont(string fontPath)
        {
            if (!File.Exists(fontPath))
            {
                MelonLoader.MelonLogger.Warning($"Font file not found: {fontPath}");
                return Font.CreateDynamicFontFromOSFont("Arial", 16);
            }

            try
            {
                Font font = Font.CreateDynamicFontFromOSFont(new[] { fontPath }, 16);

                if (font != null)
                {
                    MelonLoader.MelonLogger.Msg($"Loaded font from: {fontPath}");
                    return font;
                }
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Warning($"Failed to load font from {fontPath}: {ex.Message}");
            }

            return Font.CreateDynamicFontFromOSFont("Arial", 16);
        }

        public static Font[] LoadFontsFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Directory not found: {directoryPath}");

            string[] fontFiles = Directory.GetFiles(directoryPath, "*.ttf");
            string[] otfFiles = Directory.GetFiles(directoryPath, "*.otf");
            string[] allFiles = new string[fontFiles.Length + otfFiles.Length];
            Array.Copy(fontFiles, allFiles, fontFiles.Length);
            Array.Copy(otfFiles, 0, allFiles, fontFiles.Length, otfFiles.Length);

            List<Font> fonts = new List<Font>();
            foreach (string file in allFiles)
            {
                Font font = LoadFont(file);
                if (font != null)
                    fonts.Add(font);
            }

            return fonts.ToArray();
        }

        public static void DestroyAll()
        {
            foreach (iOverlayText text in texts)
                text.Destroy();
            texts.Clear();
            UnityEngine.Object.Destroy(CanvasObject);
        }
    }
}

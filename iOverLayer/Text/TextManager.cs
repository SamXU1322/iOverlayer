using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using TMPro;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        private static Dictionary<int,Text> _texts;
        private static int _textCount = 0;
        public static int Create(string information)
        {
            if (_texts == null)
            {
                _texts = new Dictionary<int, Text>();
            }

            if (Canvas.Root == null)
            {
                LogSystem.Error("Canvas.Root is null. Call Canvas.Init() first.");
                return -1;
            }

            GameObject prefab = AssetLoader.LoadPrefabAssetBundle("textprefab", "Text");
            if (prefab == null)
            {
                LogSystem.Error("Text prefab load failed.");
                return -1;
            }

            GameObject instance = Object.Instantiate(prefab, Canvas.Root.transform);
            Text text = instance.AddComponent<Text>();
            text.SetText(information);
            text.SetId(_textCount);
            _textCount++;
            _texts[text.ID] = text; 
            LogSystem.Info($"Text with ID {text.ID} created.");
            return text.ID;
        }
        public static void Delete(int index)
        {
            if (_texts.ContainsKey(index))
            {
                _texts.Remove(index);
            }
            else
            {
                LogSystem.Error($"Text with ID {index} does not exist.");
            }
        }
        public static void SetText(int index, string text)
        {
            _texts[index].SetText(text);
        }
        public static void SetColor(int index, Color color)
        {
            _texts[index].SetColor(color);
        }
        public static void SetFont(int index, string FontName)
        {
            _texts[index].SetFont(FontName);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        private static Dictionary<string,Text> _texts;
        private static int _textCount = 0;
        public static Dictionary<string, Text> Texts => _texts;
        public static int Create(string TextName ,string information)
        {
            if (_texts == null)
            {
                _texts = new Dictionary<string, Text>();
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
            _texts[TextName] = text; 
            LogSystem.Info($"{TextName} created.");
            return text.ID;
        }
        public static void Delete(string TextName)
        {
            if (_texts.ContainsKey(TextName))
            {
                _texts.Remove(TextName);
            }
            else
            {
                LogSystem.Error($"Text with ID {TextName} does not exist.");
            }
        }
        public static void SetText(string TextName, string text)
        {
            _texts[TextName].SetText(text);
        }
        public static void SetColor(string TextName, Color color)
        {
            _texts[TextName].SetColor(color);
        }
        public static void SetFont(string TextName, string FontName)
        {
            _texts[TextName].SetFont(FontName);
        }
    }
}
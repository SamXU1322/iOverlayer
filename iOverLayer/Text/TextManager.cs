using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        private static Dictionary<int,Text> _texts;
        private static int _textCount = 0;
        public static void Create()
        {
            if (_texts == null)
            {
                _texts = new Dictionary<int, Text>();
            }

            if (Canvas.Root == null)
            {
                LogSystem.Error("Canvas.Root is null. Call Canvas.Init() first.");
                return;
            }

            GameObject prefab = AssetLoader.LoadPrefabAssetBundle("textprefab", "Text");
            if (prefab == null)
            {
                LogSystem.Error("Text prefab load failed.");
                return;
            }

            GameObject instance = Object.Instantiate(prefab, Canvas.Root.transform);
            Text text = instance.AddComponent<Text>();
            text.setId(_textCount);
            _textCount++;
            _texts[text.ID] = text; 
            LogSystem.Info($"Text with ID {text.ID} created.");
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
        
    }
}
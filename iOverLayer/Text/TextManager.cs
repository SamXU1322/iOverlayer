using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        private static Dictionary<int,Text> _texts;
        private static int _textCount = 0;
        public static void Create()
        {
            GameObject TextInstance = AssetLoader.LoadPrefabAssetBundle("","");
            Text text = TextInstance.AddComponent<Text>();
            text.setId(_textCount);
            _textCount++;
            _texts[text.ID] = text; 
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
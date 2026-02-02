using System.Linq;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        private static Text[] _texts;
        private static int _textCount = 0;
        public static void Create()
        {
            GameObject TextInstance = AssetLoader.LoadAssetBundle("","");
            Text text = TextInstance.AddComponent<Text>();
            text.setId(_textCount);
            _textCount++;
            _texts.Append(text);
        }
    }
}
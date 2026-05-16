using UnityEngine;
using UnityEngine.UI;

namespace iOverlayer
{
    public class iOverlayText
    {
        public GameObject GameObject { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public Text TextComponent { get; private set; }

        internal iOverlayText(GameObject gameObject)
        {
            GameObject = gameObject;
            RectTransform = gameObject.GetComponent<RectTransform>();
            TextComponent = gameObject.GetComponent<Text>();
        }

        public void SetText(string text)
        {
            TextComponent.text = text;
        }

        public void SetPosition(float x, float y)
        {
            RectTransform.anchoredPosition = new Vector2(x, y);
        }

        public void SetFontSize(int size)
        {
            TextComponent.fontSize = size;
        }

        public void SetColor(Color color)
        {
            TextComponent.color = color;
        }

        public void SetFont(string fontName)
        {
            TextComponent.font = UIModule.LoadFont(fontName);
        }

        public void SetFont(Font font)
        {
            TextComponent.font = font;
        }

        public void Destroy()
        {
            Object.Destroy(GameObject);
        }
    }
}

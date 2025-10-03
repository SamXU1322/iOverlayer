using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace iOverlayer
{
    internal class TextBehavior : MonoBehaviour
    {
        public GameObject TextObject;
        public TextMeshProUGUI text;
        public Shadow shadowText;
        public RectTransform rectTransform;

         public void setSize(int size)
        {
            text.fontSize = size;
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        }
        public void setText(string text) {
            this.text.text= text;
        }

        public void setPosition(float x, float y)
        {
            Vector2 pos = new Vector2(x, y);
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;
        }

        void Awake()
        {
            // 创建Canvas
            Canvas mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // 创建Text
            TextObject = new GameObject();
            TextObject.transform.SetParent(transform);
            TextObject.AddComponent<Canvas>();
            rectTransform = TextObject.GetComponent<RectTransform>();
            
            GameObject textObject = new GameObject();
            textObject.transform.SetParent(TextObject.transform);
            
            text = textObject.AddComponent<TextMeshProUGUI>();
            text.font = TMP_Settings.defaultFontAsset;
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 32;
            text.color = Color.cyan;
            text.overflowMode = TextOverflowModes.Overflow;
            
            shadowText = textObject.AddComponent<Shadow>();
            shadowText.effectColor = new Color(0f, 0f, 0f, 0.5f);
            shadowText.effectDistance = new Vector2(2f, -2f);

            Vector2 pos = new Vector2(0, 0);
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;
            rectTransform.anchoredPosition = Vector2.zero;

        }

       
    }
}

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
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = 0.5f;
            
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
            text.color = Color.white;
            text.overflowMode = TextOverflowModes.Overflow;
            text.enableWordWrapping = false;
            
            text.fontSharedMaterial = Instantiate(text.font.material);
            text.fontSharedMaterial.SetColor(ShaderUtilities.ID_UnderlayColor,new Color32(0,0,0,80));
            text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.5f);
            text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.5f);
            text.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
            
            
            Vector2 pos = new Vector2(0, 0);
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;
            rectTransform.anchoredPosition = Vector2.zero;

        }

       
    }
}

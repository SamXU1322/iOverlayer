using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.LowLevel;

namespace iOverlayer.Core
{
    internal class TextBehavior : MonoBehaviour
    {
        public GameObject textObject;
        public TextMeshProUGUI text;
        public RectTransform rectTransform;

         public void SetSize(int newSize)
        {
            text.fontSize = newSize;
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        }
        public void SetText(string newText) {
            this.text.text= newText;
        }

        public void SetPosition(float x, float y)
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
            this.textObject = new GameObject();
            this.textObject.transform.SetParent(transform);
            this.textObject.AddComponent<Canvas>();
            rectTransform = this.textObject.GetComponent<RectTransform>();
            
            GameObject newTextObject = new GameObject();
            newTextObject.transform.SetParent(this.textObject.transform);
            
            text = newTextObject.AddComponent<TextMeshProUGUI>();
            Font DefaultFont = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            text.font = TMP_FontAsset.CreateFontAsset(DefaultFont, 100, 10, GlyphRenderMode.SDFAA, 1024, 1024);
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 32;
            text.color = Color.white;
            text.overflowMode = TextOverflowModes.Overflow;
            text.enableWordWrapping = false;
            
            text.fontMaterial = Instantiate(text.font.material);
            text.fontMaterial.SetColor(ShaderUtilities.ID_UnderlayColor,new Color32(0,0,0,80));
            text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.3f);
            text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.4f);
            text.fontMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
            
            
            Vector2 pos = new Vector2(0, 0);
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;
            rectTransform.anchoredPosition = Vector2.zero;

        }

       
    }
}
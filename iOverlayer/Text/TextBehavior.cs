using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace iOverlayer.Text
{
    internal class TextBehavior : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public RectTransform rectTransform;
        public Image image;
        public Font font;
        public TMP_FontAsset targetFont;

        public void Initialize()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            SetupDefaultProperties();
        }

        private void SetupDefaultProperties()
        {
            if (text != null)
            {
                text.enableAutoSizing = false;
                text.fontSize = 160;
                text.text = "Hello, iOverlayer!";
                text.alignment = TextAlignmentOptions.Center;
                SetFont("Default");
                text.color = Color.white;
                text.overflowMode = TextOverflowModes.Overflow;
                text.enableWordWrapping = false;
                text.ForceMeshUpdate();
                image.color = Color.clear;
                image.rectTransform.sizeDelta = new Vector2(text.preferredWidth+50, text.preferredHeight);
            }

            if (rectTransform != null)
            {
                Vector2 pos = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = pos;
                rectTransform.anchorMax = pos;
                rectTransform.pivot = pos;
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
        public void SetFont(string fontPath)
        {
            if (fontPath == "Default")
            {
                font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            }
            else
            {
                if (System.IO.File.Exists(fontPath))
                {
                    font = new Font(fontPath);
                }
            }
            targetFont = TMP_FontAsset.CreateFontAsset(font,100,10,GlyphRenderMode.SDFAA,1024,1024);
        }
        
    }
}
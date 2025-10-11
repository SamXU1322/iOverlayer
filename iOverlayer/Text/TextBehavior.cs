using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.LowLevel;


namespace iOverlayer.Text
{
    internal class TextBehavior : MonoBehaviour
    {
        public GameObject textObject;
        public TextMeshProUGUI text;
        public RectTransform rectTransform;
        public static Shader sr_msdf;
        public static Canvas mainCanvas;
        public Font font;
        public TMP_FontAsset targetFont;
        static TextBehavior()
        {
            sr_msdf = (Shader)typeof(ShaderUtilities).GetProperty("ShaderRef_MobileSDF", (BindingFlags)15420).GetValue(null);
        }
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
        public void SetColor(Color color)
        {
            text.color = color;
        }

        public void InitializedPublicCanvas()
        {
            mainCanvas = gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = 0.5f;
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
                else
                {
                    font = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
                }
            }
            targetFont = TMP_FontAsset.CreateFontAsset(font,100,10,GlyphRenderMode.SDFAA,1024,1024);
            InitMaterial(targetFont.material);
            ApplyMaterial(targetFont.material);
            text.font = targetFont;
        }

        private static void InitMaterial(Material mat)
        {
            if (sr_msdf) mat.shader = sr_msdf;
            mat.EnableKeyword(ShaderUtilities.Keyword_Outline);
            mat.EnableKeyword(ShaderUtilities.Keyword_Underlay);
        }

        private void ApplyMaterial(Material mat)
        {
            mat.SetColor(ShaderUtilities.ID_UnderlayColor,new Color32(0,0,0,80));
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.3f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.4f);
            mat.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
        }
        void Awake()
        {
            InitializedPublicCanvas();
            this.textObject = new GameObject();
            this.textObject.transform.SetParent(transform);
            this.textObject.AddComponent<Canvas>();
            rectTransform = this.textObject.GetComponent<RectTransform>();
            GameObject newTextObject = new GameObject();
            newTextObject.transform.SetParent(this.textObject.transform);
            text = newTextObject.AddComponent<TextMeshProUGUI>();
            Font defaultFont = RDString.GetFontDataForLanguage(SystemLanguage.English).font;
            TMP_FontAsset defaultTMPFont = TMP_FontAsset.CreateFontAsset(defaultFont,100,10,GlyphRenderMode.SDFAA,1024,1024);
            SetFont("Default");
            text.text = "Congratulation";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 32;
            text.color = Color.white;
            text.overflowMode = TextOverflowModes.Overflow;
            text.enableWordWrapping = false;
            Vector2 pos = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = pos;
            rectTransform.anchorMax = pos;
            rectTransform.pivot = pos;
            rectTransform.anchoredPosition = Vector2.zero;
        }
    }
}
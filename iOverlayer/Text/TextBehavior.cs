using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;


namespace iOverlayer.Text
{
    internal class TextBehavior : MonoBehaviour
    {
        public GameObject textObject;
        public TextMeshProUGUI text;
        public RectTransform rectTransform;
        public static Shader Shader;
        public static Canvas mainCanvas;
        public Font font;
        public TMP_FontAsset targetFont;
        static TextBehavior()
        {
            Shader = (Shader)typeof(ShaderUtilities).GetProperty("ShaderRef_MobileSDF", (BindingFlags)15420).GetValue(null);
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

        private static void InitMaterial(Material material)
        {
            if (Shader) material.shader = Shader;
            material.EnableKeyword(ShaderUtilities.Keyword_Outline);
            material.EnableKeyword(ShaderUtilities.Keyword_Underlay);
        }

        private void ApplyMaterial(Material material)
        {
            material.SetColor(ShaderUtilities.ID_UnderlayColor,new Color32(0,0,0,80));
            material.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, 0.3f);
            material.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, -0.4f);
            material.SetFloat(ShaderUtilities.ID_UnderlayDilate, 0);
        }
        void Awake()
        {
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
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace iOverlayer.Text
{
    public class TextBehavior : MonoBehaviour,IPointerDownHandler, IPointerUpHandler,IDragHandler
    {
        public TextMeshProUGUI text;
        public RectTransform rectTransform;
        public Image image;
        public Font font;
        public TMP_FontAsset targetFont;
        public bool isVisible;
        private Vector2 _pointerOffset;
        public bool isDragging = false;

        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            SetupDefaultProperties();
            isVisible = this.gameObject.activeSelf;
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
                image.raycastTarget = true;
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

        public void ChangeVisibility()
        {
            isVisible = !isVisible;
            this.gameObject.SetActive(isVisible);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform,
                    eventData.position,
                    eventData.pressEventCamera,
                    out _pointerOffset
                    );
                isDragging = true;
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {

            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDragging && eventData.button == PointerEventData.InputButton.Left)
            {
                if (rectTransform != null)
                {
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform.parent as RectTransform,
                            eventData.position,
                            eventData.pressEventCamera,
                            out localPoint))
                    {
                        rectTransform.anchoredPosition = localPoint - _pointerOffset;
                    }
                }
            }
                
            
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            isDragging = false;
        }
    }
}
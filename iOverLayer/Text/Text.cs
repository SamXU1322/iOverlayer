using TMPro;
using UnityEngine;
namespace iOverLayer.Text
{
    public class Text : MonoBehaviour
    {
        private int _id;
        private bool _Editmode = true;
        private bool _isDragging = false;
        private TextMeshProUGUI _textMesh;
        private RectTransform _rectTransform;
        private Vector2 _dragOffset;
        public int ID => _id;
        public void Awake()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>(true);
            _rectTransform = _textMesh.rectTransform;
            if (_textMesh == null)
            {
                LogSystem.Error("TextMeshProUGUI component not found on Text GameObject.");
                return;
            }
            TextDefaultSetting.SetDefaultText(ref _textMesh, "Asset");
            UpdateHitArea();
        }
        public void SetId(int id)
        {
            _id = id;
        }
        public void SetText(string text)
        {
            _textMesh.text = text;
            UpdateHitArea();
        }
        public void SetColor(Color color)
        {
            _textMesh.color = color;
        }
        public void SetFont(string FontName)
        {
            if (TextFont.FontAsset.ContainsKey(FontName))
            {
                _textMesh.font = TextFont.FontAsset[FontName];
                UpdateHitArea();
            }
            else
            {
                // 输出提示
            }
        }
        public void Update()
        {
            if (!_Editmode || _rectTransform == null) return;

            Vector2 mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
                int charIndex = TMP_TextUtilities.FindIntersectingCharacter(_textMesh, mousePos, null, true);
                if (charIndex != -1)
                {
                    LogSystem.Info($"Text {_id} clicked.");
                    _isDragging = true;

                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _rectTransform,
                        mousePos,
                        null,
                        out _dragOffset
                    );
                }
            }

            if (Input.GetMouseButton(0) && _isDragging)
            {
                RectTransform parentRect = _rectTransform.parent as RectTransform;
                if (parentRect == null) return;

                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    parentRect,
                    mousePos,
                    null,
                    out localPoint))
                {
                    _rectTransform.anchoredPosition = localPoint - _dragOffset;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }
        }
        private void UpdateHitArea()
        {
            if (_textMesh == null || _rectTransform == null) return;
            Vector2 preferred = _textMesh.GetPreferredValues(_textMesh.text);
            _rectTransform.sizeDelta = preferred;
            LogSystem.Info($"{preferred}");
        }
    }
}
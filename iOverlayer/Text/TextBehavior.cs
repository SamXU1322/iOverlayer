using System;
using System.Collections.Generic;
using iOverlayer.Tags;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace iOverlayer.Text
{
    public class TextBehavior : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public RectTransform rectTransform;
        public Image image;
        public Font font;
        public TMP_FontAsset targetFont;
        public bool isVisible;
        private Vector2 _pointerOffset;
        public bool isDragging = false;
        public bool isSelecting = false;
        public Image borderImage;
        private Color borderColor = new Color(0.3f, 0.6f, 1f, 1f);
        private bool _isMouseDown = false;
        private Vector3 _lastMousePosition;

        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            SetupDefaultProperties();
            this.gameObject.SetActive(false);
            isVisible = false;
        }
        private void SetupDefaultProperties()
        {
            if (text != null)
            {
                text.enableAutoSizing = false;
                text.fontSize = 160;
                text.text = "";
                text.alignment = TextAlignmentOptions.Center;
                SetFont("Default");
                text.color = Color.white;
                text.overflowMode = TextOverflowModes.Overflow;
                text.enableWordWrapping = false;
                text.raycastTarget = false;
                text.ForceMeshUpdate();
                image.color = Color.clear;
                image.raycastTarget = false; // 禁用raycastTarget，避免拦截鼠标事件
                image.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
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
        public void SetText(string text)
        {
            if (text != null)
            {
                this.text.text = text;
                image.rectTransform.sizeDelta = new Vector2(this.text.preferredWidth, this.text.preferredHeight);
            }
        }
        public void ChangeVisibility()
        {
            isVisible = !isVisible;
            this.gameObject.SetActive(isVisible);
        }

        private void Update()
        {
            // 检测鼠标左键按下
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverGameObject())
                {
                    // 鼠标在UI上，切换选择状态
                    isSelecting = !isSelecting;
                    _isMouseDown = true;
                    _lastMousePosition = Input.mousePosition;
                    
                    // 计算鼠标相对于UI元素的偏移
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform,
                        Input.mousePosition,
                        null,
                        out _pointerOffset
                    );
                }
                else if (isSelecting)
                {
                    // 鼠标不在UI上但当前处于选择状态，取消选择
                    isSelecting = false;
                    isDragging = false;
                }
            }
            
            // 检测鼠标左键抬起
            if (Input.GetMouseButtonUp(0))
            {
                _isMouseDown = false;
                isDragging = false;
            }
            
            // 处理拖动
            if (_isMouseDown && isSelecting)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 mouseDelta = currentMousePosition - _lastMousePosition;
                
                // 如果鼠标移动超过一定阈值，开始拖动
                if (mouseDelta.magnitude > 1f && !isDragging)
                {
                    isDragging = true;
                }
                
                if (isDragging && rectTransform != null)
                {
                    Vector2 localPoint;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            rectTransform.parent as RectTransform,
                            currentMousePosition,
                            null,
                            out localPoint))
                    {
                        rectTransform.anchoredPosition = localPoint - _pointerOffset;
                    }
                }
                
                _lastMousePosition = currentMousePosition;
            }
        }
        
        private bool IsPointerOverGameObject()
        {
            // 使用RectTransformUtility直接检查鼠标是否在矩形内，不依赖EventSystem的Raycast
            if (rectTransform != null)
            {
                Vector2 screenPoint = Input.mousePosition;
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        rectTransform,
                        screenPoint,
                        null,
                        out localPoint))
                {
                    // 检查点是否在rectTransform的边界内
                    Rect rect = rectTransform.rect;
                    return rect.Contains(localPoint);
                }
            }
            return false;
        }
        
        private void Deselect()
        {
            isSelecting = false;
            isDragging = false;
            _isMouseDown = false;
        }
    }
}
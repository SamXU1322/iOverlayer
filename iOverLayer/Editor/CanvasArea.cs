using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using iOverlayer.Config;

namespace iOverlayer.Editor
{
    public class CanvasArea
    {
        public event System.Action<EditorTool> ToolChanged;
        public event System.Action<VisualElement> SelectionChanged;
        public event System.Action ContentChanged;

        private VisualElement _canvasWrap;
        private VisualElement _canvas;
        private VisualElement _selectedElement;
        private EditorTool _currentTool;

        private bool _isDragging;
        private VisualElement _dragElement;
        private Vector2 _dragStartMouse;
        private Vector2 _dragStartPos;
        private bool _justCreatedText;

        private VisualElement _resizeHandle;
        private bool _isResizing;
        private Vector2 _resizeStartMouse;
        private float _resizeStartWidth;

        public VisualElement Wrap => _canvasWrap;
        public VisualElement Canvas => _canvas;

        public EditorTool CurrentTool
        {
            get => _currentTool;
            set
            {
                if (_currentTool == value) return;
                _currentTool = value;
                ToolChanged?.Invoke(value);
            }
        }

        public void Bind(VisualElement root)
        {
            _canvasWrap = root.Q<VisualElement>("canvas-wrap");
            _canvas = root.Q<VisualElement>("canvas");
            RegisterCallbacks();
        }

        public void Unbind()
        {
            ClearSelection();
            UnregisterCallbacks();
            _canvasWrap = null;
            _canvas = null;
            _dragElement = null;
            _isDragging = false;
            _isResizing = false;
        }

        private void RegisterCallbacks()
        {
            _canvas.RegisterCallback<ClickEvent>(OnCanvasClick);
            _canvas.RegisterCallback<PointerDownEvent>(OnCanvasPointerDown);
            _canvas.RegisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _canvas.RegisterCallback<PointerUpEvent>(OnCanvasPointerUp);
        }

        private void UnregisterCallbacks()
        {
            if (_canvas == null) return;
            _canvas.UnregisterCallback<ClickEvent>(OnCanvasClick);
            _canvas.UnregisterCallback<PointerDownEvent>(OnCanvasPointerDown);
            _canvas.UnregisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _canvas.UnregisterCallback<PointerUpEvent>(OnCanvasPointerUp);
        }

        private void OnCanvasClick(ClickEvent evt)
        {
            switch (CurrentTool)
            {
                case EditorTool.Text:
                    HandleTextClick(evt);
                    break;
                case EditorTool.Select:
                    HandleSelectClick(evt);
                    break;
            }
        }

        private void HandleTextClick(ClickEvent evt)
        {
            var target = evt.target as VisualElement;
            if (target is Label label && label.parent == _canvas)
            {
                CurrentTool = EditorTool.Select;
                SelectElement(label);
            }
            else
            {
                _justCreatedText = true;
                CreateText(evt.localPosition);
            }
        }

        private void HandleSelectClick(ClickEvent evt)
        {
            var target = evt.target as VisualElement;
            if (target is Label label && label.parent == _canvas)
            {
                SelectElement(label);
            }
            else if (target != _resizeHandle)
            {
                ClearSelection();
            }
        }

        private void CreateText(Vector2 position)
        {
            var label = new Label("new text");
            label.style.position = Position.Absolute;
            label.style.left = position.x;
            label.style.top = position.y;
            label.style.fontSize = 32;
            label.style.color = Color.black;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.paddingLeft = 4;
            label.style.paddingRight = 4;
            label.style.paddingTop = 2;
            label.style.paddingBottom = 2;

            ApplyFont(label, "Arial");
            _canvas.Add(label);

            CurrentTool = EditorTool.Select;
            SelectElement(label);
            ContentChanged?.Invoke();
        }

        public List<OverlayConfig> GetOverlayConfigs()
        {
            var configs = new List<OverlayConfig>();
            foreach (var child in _canvas.Children())
            {
                if (child is Label label)
                {
                    var w = label.style.width;
                    var explicitWidth = (w.keyword == StyleKeyword.Undefined || w.keyword == StyleKeyword.Auto)
                        ? 0f : w.value.value;
                    configs.Add(new OverlayConfig
                    {
                        id = System.Guid.NewGuid().ToString("N"),
                        text = label.text,
                        x = label.resolvedStyle.left,
                        y = label.resolvedStyle.top,
                        fontSize = Mathf.RoundToInt(label.resolvedStyle.fontSize),
                        color = "#" + ColorUtility.ToHtmlStringRGB(label.resolvedStyle.color),
                        font = label.userData as string ?? "Arial",
                        enabled = label.resolvedStyle.display != DisplayStyle.None,
                        width = explicitWidth
                    });
                }
            }
            return configs;
        }

        public void ClearAll()
        {
            ClearSelection();
            _canvas.Clear();
        }

        private static void ApplyFont(Label label, string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return;
            label.userData = fontName;
            try
            {
                var size = GetFontSize(label);
                var font = Font.CreateDynamicFontFromOSFont(fontName, size);
                if (font != null)
                    label.style.unityFont = font;
            }
            catch { }
        }

        private static int GetFontSize(Label label)
        {
            var fs = label.style.fontSize;
            if (fs.keyword != StyleKeyword.Undefined)
                return Mathf.Max(1, Mathf.RoundToInt(fs.value.value));
            return Mathf.Max(1, Mathf.RoundToInt(label.resolvedStyle.fontSize));
        }

        public void LoadFromConfigs(List<OverlayConfig> configs)
        {
            ClearAll();
            foreach (var config in configs)
            {
                var label = new Label(config.text);
                label.style.position = Position.Absolute;
                label.style.left = config.x;
                label.style.top = config.y;
                label.style.fontSize = config.fontSize;
                label.style.whiteSpace = WhiteSpace.Normal;
                if (config.width > 0)
                    label.style.width = config.width;

                if (ColorUtility.TryParseHtmlString(config.color, out Color c))
                    label.style.color = c;

                label.userData = config.font;
                label.style.display = config.enabled ? DisplayStyle.Flex : DisplayStyle.None;

                ApplyFont(label, config.font);
                _canvas.Add(label);
            }
        }

        private void SelectElement(VisualElement element)
        {
            ClearSelection();
            _selectedElement = element;
            _selectedElement.AddToClassList("selected");
            _selectedElement.RegisterCallback<GeometryChangedEvent>(OnSelectedGeometryChanged);

            _resizeHandle = new VisualElement();
            _resizeHandle.AddToClassList("resize-handle");
            _canvas.Add(_resizeHandle);
            UpdateResizeHandlePosition();

            _resizeHandle.RegisterCallback<PointerDownEvent>(OnResizePointerDown);
            _resizeHandle.RegisterCallback<PointerMoveEvent>(OnResizePointerMove);
            _resizeHandle.RegisterCallback<PointerUpEvent>(OnResizePointerUp);
            _resizeHandle.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());

            SelectionChanged?.Invoke(element);
        }

        private void ClearSelection()
        {
            if (_selectedElement != null)
            {
                _selectedElement.UnregisterCallback<GeometryChangedEvent>(OnSelectedGeometryChanged);
                _selectedElement.RemoveFromClassList("selected");
                _selectedElement = null;
            }
            if (_resizeHandle != null)
            {
                _resizeHandle.RemoveFromHierarchy();
                _resizeHandle = null;
            }
            SelectionChanged?.Invoke(null);
        }

        private void OnSelectedGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateResizeHandlePosition();
        }

        private void UpdateResizeHandlePosition()
        {
            if (_selectedElement == null || _resizeHandle == null) return;
            var s = _selectedElement.resolvedStyle;
            // Place handle at bottom-right corner, half-overlapping the border
            _resizeHandle.style.left = s.left + s.width - 5;
            _resizeHandle.style.top = s.top + s.height - 5;
        }

        private void OnResizePointerDown(PointerDownEvent evt)
        {
            if (_selectedElement == null) return;
            _isResizing = true;
            _resizeStartMouse = evt.position;
            var w = _selectedElement.style.width;
            _resizeStartWidth = (w.keyword == StyleKeyword.Undefined || w.keyword == StyleKeyword.Auto)
                ? _selectedElement.resolvedStyle.width
                : w.value.value;
            _resizeHandle.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnResizePointerMove(PointerMoveEvent evt)
        {
            if (!_isResizing || _selectedElement == null) return;
            if (!_resizeHandle.HasPointerCapture(evt.pointerId)) return;
            var delta = evt.position.x - _resizeStartMouse.x;
            _selectedElement.style.width = Mathf.Max(20f, _resizeStartWidth + delta);
            evt.StopPropagation();
        }

        private void OnResizePointerUp(PointerUpEvent evt)
        {
            if (_isResizing)
            {
                if (_resizeHandle != null && _resizeHandle.HasPointerCapture(evt.pointerId))
                    _resizeHandle.ReleasePointer(evt.pointerId);
                ContentChanged?.Invoke();
                if (_selectedElement != null) SelectionChanged?.Invoke(_selectedElement);
            }
            _isResizing = false;
            evt.StopPropagation();
        }

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            if (CurrentTool != EditorTool.Select) return;
            if (evt.target == _resizeHandle) return;

            var target = evt.target as VisualElement;
            if (target is Label label && label.parent == _canvas && label.ClassListContains("selected") && !_justCreatedText)
            {
                _isDragging = true;
                _dragElement = label;
                _dragStartMouse = evt.localPosition;
                _dragStartPos = new Vector2(label.resolvedStyle.left, label.resolvedStyle.top);
                _dragElement.BringToFront();
                if (_resizeHandle != null) _resizeHandle.BringToFront();
                evt.StopPropagation();
            }
        }

        private void OnCanvasPointerMove(PointerMoveEvent evt)
        {
            if (!_isDragging || _dragElement == null) return;
            var delta = (Vector2)evt.localPosition - _dragStartMouse;
            _dragElement.style.left = _dragStartPos.x + delta.x;
            _dragElement.style.top = _dragStartPos.y + delta.y;
        }

        private void OnCanvasPointerUp(PointerUpEvent evt)
        {
            if (_isDragging && _dragElement != null)
            {
                SelectionChanged?.Invoke(_dragElement);
                ContentChanged?.Invoke();
            }
            _isDragging = false;
            _dragElement = null;
            _justCreatedText = false;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using iOverlayer.Config;

namespace iOverlayer.Editor
{
    public enum ResizeDir { Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight }

    public class CanvasArea
    {
        public event System.Action<EditorTool> ToolChanged;
        public event System.Action<VisualElement> SelectionChanged;
        public event System.Action ContentChanged;
        public event System.Action OverlaysChanged;

        private int _textCounter;

        private VisualElement _canvasWrap;
        private VisualElement _canvas;
        private VisualElement _selectedElement;
        private EditorTool _currentTool;

        private bool _isDragging;
        private VisualElement _dragElement;
        private Vector2 _dragStartMouse;
        private Vector2 _dragStartPos;
        private bool _justCreatedText;

        private readonly VisualElement[] _resizeHandles = new VisualElement[8];
        private ResizeDir _activeDir;
        private bool _isResizing;
        private Vector2 _resizeStartMouse;
        private Vector2 _resizeStartPos;
        private Vector2 _resizeStartSize;

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
                if (!LabelData.Of(label).locked)
                {
                    SelectElement(label);
                }
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
                if (!LabelData.Of(label).locked)
                    SelectElement(label);
            }
            else if (!IsResizeHandle(target))
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
            label.style.color = Color.white;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.paddingLeft = 4;
            label.style.paddingRight = 4;
            label.style.paddingTop = 2;
            label.style.paddingBottom = 2;

            var data = LabelData.Of(label);
            data.name = "文本 " + (++_textCounter);
            ApplyFont(label, "Arial");
            _canvas.Add(label);

            SelectElement(label);
            ContentChanged?.Invoke();
            OverlaysChanged?.Invoke();
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
                    var data = LabelData.Of(label);
                    configs.Add(new OverlayConfig
                    {
                        id = System.Guid.NewGuid().ToString("N"),
                        name = data.name,
                        text = label.text,
                        x = label.resolvedStyle.left,
                        y = label.resolvedStyle.top,
                        fontSize = Mathf.RoundToInt(label.resolvedStyle.fontSize),
                        color = "#" + ColorUtility.ToHtmlStringRGB(label.resolvedStyle.color),
                        font = data.font ?? "Arial",
                        enabled = label.style.display != DisplayStyle.None,
                        locked = data.locked,
                        hidden = data.hidden,
                        width = explicitWidth,
                        textAlign = data.textAlign.ToString()
                    });
                }
            }
            return configs;
        }

        public void ClearAll()
        {
            ClearSelection();
            _canvas.Clear();
            _textCounter = 0;
            OverlaysChanged?.Invoke();
        }

        public List<Label> GetLabels()
        {
            var labels = new List<Label>();
            foreach (var child in _canvas.Children())
                if (child is Label label) labels.Add(label);
            return labels;
        }

        public Label SelectedLabel => _selectedElement as Label;

        public void SelectLabel(Label label)
        {
            CurrentTool = EditorTool.Select;
            SelectElement(label);
        }

        public void RenameLabel(Label label, string newName)
        {
            LabelData.Of(label).name = newName;
            ContentChanged?.Invoke();
            OverlaysChanged?.Invoke();
        }

        public void SetLabelLocked(Label label, bool locked)
        {
            LabelData.Of(label).locked = locked;
            if (locked && _selectedElement == label)
                ClearSelection();
            ContentChanged?.Invoke();
            OverlaysChanged?.Invoke();
        }

        public void SetLabelVisible(Label label, bool visible)
        {
            LabelData.Of(label).hidden = !visible;
            label.style.visibility = visible ? Visibility.Visible : Visibility.Hidden;
            if (_selectedElement == label && _selectedElement != null)
                SelectionChanged?.Invoke(_selectedElement);
            ContentChanged?.Invoke();
            OverlaysChanged?.Invoke();
        }

        public void DeleteLabel(Label label)
        {
            if (_selectedElement == label)
                ClearSelection();
            label.RemoveFromHierarchy();
            ContentChanged?.Invoke();
            OverlaysChanged?.Invoke();
        }

        private static void ApplyFont(Label label, string fontName)
        {
            if (string.IsNullOrEmpty(fontName)) return;
            LabelData.Of(label).font = fontName;
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

                var data = LabelData.Of(label);
                data.name = string.IsNullOrEmpty(config.name) ? "文本 " + (++_textCounter) : config.name;
                data.font = config.font;
                data.locked = config.locked;
                data.hidden = config.hidden;
                if (!string.IsNullOrEmpty(config.textAlign) && System.Enum.TryParse(config.textAlign, out TextAnchor anchor))
                {
                    data.textAlign = anchor;
                    label.style.unityTextAlign = anchor;
                }
                label.style.display = config.enabled ? DisplayStyle.Flex : DisplayStyle.None;
                label.style.visibility = config.hidden ? Visibility.Hidden : Visibility.Visible;

                ApplyFont(label, config.font);
                _canvas.Add(label);
            }
            OverlaysChanged?.Invoke();
        }

        private void SelectElement(VisualElement element)
        {
            ClearSelection();
            _selectedElement = element;
            _selectedElement.AddToClassList("selected");
            _selectedElement.RegisterCallback<GeometryChangedEvent>(OnSelectedGeometryChanged);

            var dirs = (ResizeDir[])System.Enum.GetValues(typeof(ResizeDir));
            for (int i = 0; i < 8; i++)
            {
                var handle = new VisualElement();
                handle.AddToClassList("resize-handle");
                handle.userData = dirs[i];
                handle.RegisterCallback<PointerDownEvent>(OnResizePointerDown);
                handle.RegisterCallback<PointerMoveEvent>(OnResizePointerMove);
                handle.RegisterCallback<PointerUpEvent>(OnResizePointerUp);
                handle.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
                _canvas.Add(handle);
                _resizeHandles[i] = handle;
            }
            UpdateResizeHandlePositions();

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
            for (int i = 0; i < 8; i++)
            {
                if (_resizeHandles[i] != null)
                {
                    _resizeHandles[i].RemoveFromHierarchy();
                    _resizeHandles[i] = null;
                }
            }
            SelectionChanged?.Invoke(null);
        }

        private bool IsResizeHandle(VisualElement target)
        {
            return target != null && target.ClassListContains("resize-handle");
        }

        private void OnSelectedGeometryChanged(GeometryChangedEvent evt)
        {
            UpdateResizeHandlePositions();
        }

        private void UpdateResizeHandlePositions()
        {
            if (_selectedElement == null) return;
            var s = _selectedElement.resolvedStyle;
            float l = s.left, t = s.top, w = s.width, h = s.height;
            float cx = l + w * 0.5f, cy = t + h * 0.5f;

            // Positions for each dir: Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight
            Vector2[] pos =
            {
                new Vector2(l - 5, cy - 5),
                new Vector2(cx - 5, t - 5),
                new Vector2(l + w - 5, cy - 5),
                new Vector2(cx - 5, t + h - 5),
                new Vector2(l - 5, t - 5),
                new Vector2(l + w - 5, t - 5),
                new Vector2(l - 5, t + h - 5),
                new Vector2(l + w - 5, t + h - 5)
            };

            for (int i = 0; i < 8; i++)
            {
                if (_resizeHandles[i] == null) continue;
                _resizeHandles[i].style.left = pos[i].x;
                _resizeHandles[i].style.top = pos[i].y;
            }
        }

        private void OnResizePointerDown(PointerDownEvent evt)
        {
            if (_selectedElement == null) return;
            var handle = evt.currentTarget as VisualElement;
            _activeDir = (ResizeDir)handle.userData;
            _isResizing = true;
            _resizeStartMouse = evt.position;
            _resizeStartPos = new Vector2(_selectedElement.resolvedStyle.left, _selectedElement.resolvedStyle.top);
            _resizeStartSize = new Vector2(_selectedElement.resolvedStyle.width, _selectedElement.resolvedStyle.height);
            handle.CapturePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void OnResizePointerMove(PointerMoveEvent evt)
        {
            if (!_isResizing || _selectedElement == null) return;
            var dx = evt.position.x - _resizeStartMouse.x;
            var dy = evt.position.y - _resizeStartMouse.y;

            float l = _resizeStartPos.x, t = _resizeStartPos.y;
            float w = _resizeStartSize.x, h = _resizeStartSize.y;
            const float min = 20f;

            bool affectsL = _activeDir == ResizeDir.Left || _activeDir == ResizeDir.TopLeft || _activeDir == ResizeDir.BottomLeft;
            bool affectsR = _activeDir == ResizeDir.Right || _activeDir == ResizeDir.TopRight || _activeDir == ResizeDir.BottomRight;
            bool affectsT = _activeDir == ResizeDir.Top || _activeDir == ResizeDir.TopLeft || _activeDir == ResizeDir.TopRight;
            bool affectsB = _activeDir == ResizeDir.Bottom || _activeDir == ResizeDir.BottomLeft || _activeDir == ResizeDir.BottomRight;

            if (affectsL) { float nw = w - dx; if (nw >= min) { l += dx; w = nw; } }
            if (affectsR) { float nw = w + dx; if (nw >= min) w = nw; }
            if (affectsT) { float nh = h - dy; if (nh >= min) { t += dy; h = nh; } }
            if (affectsB) { float nh = h + dy; if (nh >= min) h = nh; }

            _selectedElement.style.left = l;
            _selectedElement.style.top = t;
            _selectedElement.style.width = w;
            _selectedElement.style.height = h;
            evt.StopPropagation();
        }

        private void OnResizePointerUp(PointerUpEvent evt)
        {
            if (_isResizing)
            {
                for (int i = 0; i < 8; i++)
                    if (_resizeHandles[i] != null && _resizeHandles[i].HasPointerCapture(evt.pointerId))
                        _resizeHandles[i].ReleasePointer(evt.pointerId);
                ContentChanged?.Invoke();
                if (_selectedElement != null) SelectionChanged?.Invoke(_selectedElement);
            }
            _isResizing = false;
            evt.StopPropagation();
        }

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            if (CurrentTool != EditorTool.Select) return;
            if (IsResizeHandle(evt.target as VisualElement)) return;

            var target = evt.target as VisualElement;
            if (target is Label label && label.parent == _canvas && label.ClassListContains("selected") && !_justCreatedText)
            {
                _isDragging = true;
                _dragElement = label;
                _dragStartMouse = evt.localPosition;
                _dragStartPos = new Vector2(label.resolvedStyle.left, label.resolvedStyle.top);
                _dragElement.BringToFront();
                BringHandlesToFront();
                evt.StopPropagation();
            }
        }

        private void BringHandlesToFront()
        {
            for (int i = 0; i < 8; i++)
                if (_resizeHandles[i] != null) _resizeHandles[i].BringToFront();
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

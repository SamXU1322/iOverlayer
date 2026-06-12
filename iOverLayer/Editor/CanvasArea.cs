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
            UnregisterCallbacks();
            _canvasWrap = null;
            _canvas = null;
            _selectedElement = null;
            _dragElement = null;
            _isDragging = false;
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
            else
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
                    configs.Add(new OverlayConfig
                    {
                        id = System.Guid.NewGuid().ToString("N"),
                        text = label.text,
                        x = label.resolvedStyle.left,
                        y = label.resolvedStyle.top,
                        fontSize = Mathf.RoundToInt(label.resolvedStyle.fontSize),
                        color = "#" + ColorUtility.ToHtmlStringRGB(label.resolvedStyle.color),
                        font = label.userData as string ?? "Arial",
                        enabled = label.resolvedStyle.display != DisplayStyle.None
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
            SelectionChanged?.Invoke(element);
        }

        private void ClearSelection()
        {
            if (_selectedElement != null)
            {
                _selectedElement.RemoveFromClassList("selected");
                _selectedElement = null;
            }
            SelectionChanged?.Invoke(null);
        }

        private void OnCanvasPointerDown(PointerDownEvent evt)
        {
            if (CurrentTool != EditorTool.Select) return;

            var target = evt.target as VisualElement;
            if (target is Label label && label.parent == _canvas && label.ClassListContains("selected"))
            {
                _isDragging = true;
                _dragElement = label;
                _dragStartMouse = evt.localPosition;
                _dragStartPos = new Vector2(label.resolvedStyle.left, label.resolvedStyle.top);
                _dragElement.BringToFront();
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
        }
    }
}

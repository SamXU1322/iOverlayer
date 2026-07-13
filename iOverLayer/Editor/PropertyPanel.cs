using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class PropertyPanel
    {
        public event System.Action ContentChanged;

        private Label _targetLabel;
        private bool _isUpdating;

        private VisualElement _noSelection;
        private VisualElement _propFields;
        private TextField _propText;
        private FloatField _propX;
        private FloatField _propY;
        private IntegerField _propFontSize;
        private TextField _propColor;
        private VisualElement _colorSwatch;
        private VisualElement _pickerOverlay;
        private VisualElement _pickerBody;
        private Button _pickerClose;
        private VisualElement _pickerPreview;
        private TextField _pickerHex;
        private float _selectedHue;
        private float _selectedS = 1f;
        private float _selectedV = 1f;
        private VisualElement _hueBar;
        private VisualElement _svSquare;
        private Texture2D _hueTex;
        private Texture2D _svTex;
        private Button _propFont;
        private Toggle _propEnabled;

        private VisualElement _fontPickerOverlay;
        private Button _fontPickerClose;
        private TextField _fontSearch;
        private ScrollView _fontList;
        private List<string> _allFontNames = new List<string>();

        public void Bind(VisualElement root)
        {
            _noSelection = root.Q<VisualElement>("no-selection");
            _propFields = root.Q<VisualElement>("prop-fields");
            _propText = root.Q<TextField>("prop-text");
            _propX = root.Q<FloatField>("prop-x");
            _propY = root.Q<FloatField>("prop-y");
            _propFontSize = root.Q<IntegerField>("prop-fontSize");
            _propColor = root.Q<TextField>("prop-color");
            _colorSwatch = root.Q<VisualElement>("color-swatch");
            _pickerOverlay = root.Q<VisualElement>("color-picker-popup");
            _pickerBody = root.Q<VisualElement>("picker-body");
            _pickerClose = root.Q<Button>("picker-close");
            _pickerPreview = root.Q<VisualElement>("picker-preview");
            _pickerHex = root.Q<TextField>("picker-hex");
            _propFont = root.Q<Button>("prop-font");
            _propEnabled = root.Q<Toggle>("prop-enabled");

            _fontPickerOverlay = root.Q<VisualElement>("font-picker-popup");
            _fontPickerClose = root.Q<Button>("font-picker-close");
            _fontSearch = root.Q<TextField>("font-search");
            _fontList = root.Q<ScrollView>("font-list");

            PopulateFontChoices();
            RegisterCallbacks();
        }

        private void PopulateFontChoices()
        {
            var names = Font.GetOSInstalledFontNames();
            if (names != null && names.Length > 0)
            {
                _allFontNames.AddRange(names);
                _allFontNames.Sort();
            }
            else
            {
                _allFontNames.AddRange(new[] { "Arial", "Consolas", "微软雅黑", "SimSun", "Times New Roman", "Courier New" });
            }
            RebuildFontList(null);
        }

        private void RebuildFontList(string filter)
        {
            if (_fontList == null) return;
            _fontList.Clear();
            foreach (var name in _allFontNames)
            {
                if (!string.IsNullOrEmpty(filter) && name.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                var row = new VisualElement();
                row.AddToClassList("row-item");
                var label = new Label(name);
                label.AddToClassList("name");
                row.Add(label);
                row.RegisterCallback<ClickEvent>(evt =>
                {
                    ApplyFont(name);
                    _isUpdating = true;
                    if (_propFont != null) _propFont.text = name;
                    _isUpdating = false;
                    HideFontPicker();
                    ContentChanged?.Invoke();
                    evt.StopPropagation();
                });
                _fontList.Add(row);
            }
        }

        private void ApplyFont(string fontName)
        {
            if (_targetLabel == null || string.IsNullOrEmpty(fontName)) return;
            _targetLabel.userData = fontName;
            try
            {
                var size = GetFontSize(_targetLabel);
                var font = Font.CreateDynamicFontFromOSFont(fontName, size);
                if (font != null)
                {
                    _targetLabel.style.unityFont = font;
                    _targetLabel.style.unityFontDefinition = new StyleFontDefinition(FontDefinition.FromFont(font));
                }
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

        public void Unbind()
        {
            UnregisterCallbacks();
            _targetLabel = null;
            _noSelection = null;
            _propFields = null;
            _propText = null;
            _propX = null;
            _propY = null;
            _propFontSize = null;
            _propColor = null;
            _colorSwatch = null;
            _pickerOverlay = null;
            _pickerBody = null;
            _pickerClose = null;
            _pickerPreview = null;
            _pickerHex = null;
            _hueBar = null;
            _svSquare = null;
            _propFont = null;
            _propEnabled = null;
            _fontPickerOverlay = null;
            _fontPickerClose = null;
            _fontSearch = null;
            _fontList = null;

            if (_hueTex != null) { Object.Destroy(_hueTex); _hueTex = null; }
            if (_svTex != null) { Object.Destroy(_svTex); _svTex = null; }
        }

        public void SelectTarget(Label label)
        {
            _targetLabel = label;
            _isUpdating = true;

            _noSelection.style.display = DisplayStyle.None;
            _propFields.style.display = DisplayStyle.Flex;

            _propText.value = label.text;
            _propX.value = label.resolvedStyle.left;
            _propY.value = label.resolvedStyle.top;
            _propFontSize.value = Mathf.RoundToInt(label.resolvedStyle.fontSize);

            var colorHex = "#" + ColorUtility.ToHtmlStringRGB(label.resolvedStyle.color);
            _propColor.value = colorHex;
            UpdateColorSwatch(colorHex);
            HideColorPicker();

            var fontName = label.userData as string ?? "Arial";
            if (_propFont != null) _propFont.text = fontName;
            ApplyFont(fontName);

            _propEnabled.value = label.resolvedStyle.display != DisplayStyle.None;

            _isUpdating = false;
        }

        public void ClearTarget()
        {
            _targetLabel = null;
            _isUpdating = true;

            _noSelection.style.display = DisplayStyle.Flex;
            _propFields.style.display = DisplayStyle.None;
            HideColorPicker();
            HideFontPicker();

            _isUpdating = false;
        }

        private void RegisterCallbacks()
        {
            if (_propText != null)
                _propText.RegisterValueChangedCallback(OnPropTextChanged);
            if (_propX != null)
                _propX.RegisterValueChangedCallback(OnPropXChanged);
            if (_propY != null)
                _propY.RegisterValueChangedCallback(OnPropYChanged);
            if (_propFontSize != null)
                _propFontSize.RegisterValueChangedCallback(OnPropFontSizeChanged);
            if (_propColor != null)
                _propColor.RegisterValueChangedCallback(OnPropColorChanged);
            if (_colorSwatch != null)
                _colorSwatch.RegisterCallback<PointerDownEvent>(OnColorSwatchPointerDown);
            if (_pickerClose != null)
                _pickerClose.clicked += HideColorPicker;
            if (_pickerHex != null)
                _pickerHex.RegisterValueChangedCallback(OnPickerHexChanged);
            if (_pickerBody != null)
                BuildColorPicker();
            if (_propFont != null)
                _propFont.clicked += OnPropFontClicked;
            if (_propEnabled != null)
                _propEnabled.RegisterValueChangedCallback(OnPropEnabledChanged);
            if (_fontPickerClose != null)
                _fontPickerClose.clicked += HideFontPicker;
            if (_fontSearch != null)
                _fontSearch.RegisterValueChangedCallback(OnFontSearchChanged);
        }

        private void UnregisterCallbacks()
        {
            if (_propText != null)
                _propText.UnregisterValueChangedCallback(OnPropTextChanged);
            if (_propX != null)
                _propX.UnregisterValueChangedCallback(OnPropXChanged);
            if (_propY != null)
                _propY.UnregisterValueChangedCallback(OnPropYChanged);
            if (_propFontSize != null)
                _propFontSize.UnregisterValueChangedCallback(OnPropFontSizeChanged);
            if (_propColor != null)
                _propColor.UnregisterValueChangedCallback(OnPropColorChanged);
            if (_colorSwatch != null)
                _colorSwatch.UnregisterCallback<PointerDownEvent>(OnColorSwatchPointerDown);
            if (_pickerClose != null)
                _pickerClose.clicked -= HideColorPicker;
            if (_pickerHex != null)
                _pickerHex.UnregisterValueChangedCallback(OnPickerHexChanged);
            if (_propFont != null)
                _propFont.clicked -= OnPropFontClicked;
            if (_propEnabled != null)
                _propEnabled.UnregisterValueChangedCallback(OnPropEnabledChanged);
            if (_fontPickerClose != null)
                _fontPickerClose.clicked -= HideFontPicker;
            if (_fontSearch != null)
                _fontSearch.UnregisterValueChangedCallback(OnFontSearchChanged);
        }

        private void OnPropTextChanged(ChangeEvent<string> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            _targetLabel.text = evt.newValue;
            ContentChanged?.Invoke();
        }

        private void OnPropXChanged(ChangeEvent<float> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            _targetLabel.style.left = evt.newValue;
            ContentChanged?.Invoke();
        }

        private void OnPropYChanged(ChangeEvent<float> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            _targetLabel.style.top = evt.newValue;
            ContentChanged?.Invoke();
        }

        private void OnPropFontSizeChanged(ChangeEvent<int> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            _targetLabel.style.fontSize = evt.newValue;
            ContentChanged?.Invoke();
        }

        private void OnPropColorChanged(ChangeEvent<string> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            ApplyColor(evt.newValue);
        }

        private void OnColorSwatchPointerDown(PointerDownEvent evt)
        {
            if (_pickerOverlay == null || _targetLabel == null) return;
            if (_pickerOverlay.style.display != DisplayStyle.None)
            {
                HideColorPicker();
                evt.StopPropagation();
                return;
            }
            var colorHex = _propColor.value;
            if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
            {
                Color.RGBToHSV(c, out float h, out float s, out float v);
                _selectedHue = s > 0.001f ? h : 0f;
                _selectedS = s;
                _selectedV = v;
            }
            BuildColorPicker();
            UpdatePickerPreview(colorHex);
            _pickerOverlay.style.display = DisplayStyle.Flex;
            evt.StopPropagation();
        }

        private void OnPickerHexChanged(ChangeEvent<string> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            ApplyColor(evt.newValue);
            UpdatePickerPreview(evt.newValue);
        }

        private void HideColorPicker()
        {
            if (_pickerOverlay != null)
                _pickerOverlay.style.display = DisplayStyle.None;
        }

        private void UpdatePickerPreview(string colorHex)
        {
            if (_pickerPreview == null) return;
            if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
                _pickerPreview.style.backgroundColor = new StyleColor(c);
        }

        private void BuildColorPicker()
        {
            if (_pickerBody == null) return;
            if (_svSquare != null)
            {
                RefreshSvTexture();
                UpdatePickerCursors();
                return;
            }

            _pickerBody.Clear();

            _svSquare = new VisualElement();
            _svSquare.AddToClassList("sv-square");
            var svCursor = new VisualElement();
            svCursor.name = "sv-cursor";
            svCursor.AddToClassList("sv-cursor");
            svCursor.pickingMode = PickingMode.Ignore;
            _svSquare.Add(svCursor);
            _svSquare.RegisterCallback<PointerDownEvent>(OnSvPointerDown);
            _svSquare.RegisterCallback<PointerMoveEvent>(OnSvPointerMove);
            _svSquare.RegisterCallback<PointerUpEvent>(OnSvPointerUp);
            _pickerBody.Add(_svSquare);

            _hueBar = new VisualElement();
            _hueBar.AddToClassList("hue-bar");
            var hueCursor = new VisualElement();
            hueCursor.name = "hue-cursor";
            hueCursor.AddToClassList("hue-cursor");
            hueCursor.pickingMode = PickingMode.Ignore;
            _hueBar.Add(hueCursor);
            _hueBar.RegisterCallback<PointerDownEvent>(OnHuePointerDown);
            _hueBar.RegisterCallback<PointerMoveEvent>(OnHuePointerMove);
            _hueBar.RegisterCallback<PointerUpEvent>(OnHuePointerUp);
            _pickerBody.Add(_hueBar);

            BuildHueTexture();
            RefreshSvTexture();
            UpdatePickerCursors();
        }

        private void BuildHueTexture()
        {
            const int h = 128;
            if (_hueTex == null)
            {
                _hueTex = new Texture2D(1, h, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
                for (int y = 0; y < h; y++)
                    _hueTex.SetPixel(0, y, Color.HSVToRGB((float)y / (h - 1), 1f, 1f));
                _hueTex.Apply();
            }
            _hueBar.style.backgroundImage = new StyleBackground(_hueTex);
        }

        private void RefreshSvTexture()
        {
            const int n = 64;
            if (_svTex == null)
                _svTex = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                    _svTex.SetPixel(x, y, Color.HSVToRGB(_selectedHue, (float)x / (n - 1), (float)y / (n - 1)));
            _svTex.Apply();
            _svSquare.style.backgroundImage = new StyleBackground(_svTex);
        }

        private void UpdatePickerCursors()
        {
            var svCursor = _svSquare?.Q<VisualElement>("sv-cursor");
            if (svCursor != null)
            {
                svCursor.style.left = new Length(_selectedS * 100f, LengthUnit.Percent);
                svCursor.style.top = new Length((1f - _selectedV) * 100f, LengthUnit.Percent);
            }
            var hueCursor = _hueBar?.Q<VisualElement>("hue-cursor");
            if (hueCursor != null)
                hueCursor.style.top = new Length((1f - _selectedHue) * 100f, LengthUnit.Percent);
        }

        private void OnSvPointerDown(PointerDownEvent evt)
        {
            _svSquare.CapturePointer(evt.pointerId);
            PickSv(evt.localPosition);
            evt.StopPropagation();
        }

        private void OnSvPointerMove(PointerMoveEvent evt)
        {
            if (!_svSquare.HasPointerCapture(evt.pointerId)) return;
            PickSv(evt.localPosition);
            evt.StopPropagation();
        }

        private void OnSvPointerUp(PointerUpEvent evt)
        {
            if (_svSquare.HasPointerCapture(evt.pointerId))
                _svSquare.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void PickSv(Vector2 local)
        {
            var w = _svSquare.contentRect.width;
            var hgt = _svSquare.contentRect.height;
            if (w <= 0 || hgt <= 0) return;
            _selectedS = Mathf.Clamp01(local.x / w);
            _selectedV = Mathf.Clamp01(1f - local.y / hgt);
            UpdatePickerCursors();
            CommitPickerColor();
        }

        private void OnHuePointerDown(PointerDownEvent evt)
        {
            _hueBar.CapturePointer(evt.pointerId);
            PickHue(evt.localPosition);
            evt.StopPropagation();
        }

        private void OnHuePointerMove(PointerMoveEvent evt)
        {
            if (!_hueBar.HasPointerCapture(evt.pointerId)) return;
            PickHue(evt.localPosition);
            evt.StopPropagation();
        }

        private void OnHuePointerUp(PointerUpEvent evt)
        {
            if (_hueBar.HasPointerCapture(evt.pointerId))
                _hueBar.ReleasePointer(evt.pointerId);
            evt.StopPropagation();
        }

        private void PickHue(Vector2 local)
        {
            var hgt = _hueBar.contentRect.height;
            if (hgt <= 0) return;
            _selectedHue = Mathf.Clamp01(1f - local.y / hgt);
            RefreshSvTexture();
            UpdatePickerCursors();
            CommitPickerColor();
        }

        private void CommitPickerColor()
        {
            if (_targetLabel == null) return;
            var c = Color.HSVToRGB(_selectedHue, _selectedS, _selectedV);
            var colorHex = "#" + ColorUtility.ToHtmlStringRGB(c);
            _isUpdating = true;
            _propColor.SetValueWithoutNotify(colorHex);
            _isUpdating = false;
            ApplyColor(colorHex);
            UpdatePickerPreview(colorHex);
        }

        private void ApplyColor(string colorHex)
        {
            UpdateColorSwatch(colorHex);
            if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
                _targetLabel.style.color = c;
            ContentChanged?.Invoke();
        }

        private void OnPropFontClicked()
        {
            if (_fontPickerOverlay == null) return;
            if (_fontPickerOverlay.style.display != DisplayStyle.None)
            {
                HideFontPicker();
                return;
            }
            if (_fontSearch != null) _fontSearch.SetValueWithoutNotify("");
            RebuildFontList(null);
            _fontPickerOverlay.style.display = DisplayStyle.Flex;
        }

        private void OnFontSearchChanged(ChangeEvent<string> evt)
        {
            RebuildFontList(evt.newValue);
        }

        private void HideFontPicker()
        {
            if (_fontPickerOverlay != null)
                _fontPickerOverlay.style.display = DisplayStyle.None;
        }

        private void OnPropEnabledChanged(ChangeEvent<bool> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            _targetLabel.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
            ContentChanged?.Invoke();
        }

        private void UpdateColorSwatch(string colorHex)
        {
            if (_colorSwatch == null) return;
            if (ColorUtility.TryParseHtmlString(colorHex, out Color c))
            {
                _colorSwatch.style.backgroundColor = new StyleColor(c);
            }
        }
    }
}

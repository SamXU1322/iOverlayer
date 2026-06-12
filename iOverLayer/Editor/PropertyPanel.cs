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
        private DropdownField _propFont;
        private Toggle _propEnabled;

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
            _propFont = root.Q<DropdownField>("prop-font");
            _propEnabled = root.Q<Toggle>("prop-enabled");

            PopulateFontChoices();
            RegisterCallbacks();
        }

        private void PopulateFontChoices()
        {
            if (_propFont == null) return;
            try
            {
                var fontNames = new List<string>();
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"))
                {
                    if (key != null)
                    {
                        var seen = new HashSet<string>();
                        foreach (var name in key.GetValueNames())
                        {
                            // "Arial (TrueType)" → "Arial"
                            var family = name;
                            var paren = family.LastIndexOf('(');
                            if (paren > 0) family = family.Substring(0, paren).Trim();
                            if (seen.Add(family)) fontNames.Add(family);
                        }
                    }
                }
                if (fontNames.Count > 0)
                    _propFont.choices = fontNames;
            }
            catch
            {
                // keep UXML default choices
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
                    _targetLabel.style.unityFont = font;
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
            _propFont = null;
            _propEnabled = null;
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

            var fontName = label.userData as string ?? "Arial";
            if (_propFont.choices.Contains(fontName))
                _propFont.value = fontName;
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
            if (_propFont != null)
                _propFont.RegisterValueChangedCallback(OnPropFontChanged);
            if (_propEnabled != null)
                _propEnabled.RegisterValueChangedCallback(OnPropEnabledChanged);
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
            if (_propFont != null)
                _propFont.UnregisterValueChangedCallback(OnPropFontChanged);
            if (_propEnabled != null)
                _propEnabled.UnregisterValueChangedCallback(OnPropEnabledChanged);
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
            UpdateColorSwatch(evt.newValue);
            if (ColorUtility.TryParseHtmlString(evt.newValue, out Color c))
                _targetLabel.style.color = c;
            ContentChanged?.Invoke();
        }

        private void OnPropFontChanged(ChangeEvent<string> evt)
        {
            if (_isUpdating || _targetLabel == null) return;
            ApplyFont(evt.newValue);
            ContentChanged?.Invoke();
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

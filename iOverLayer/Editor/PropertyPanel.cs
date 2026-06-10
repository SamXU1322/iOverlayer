using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class PropertyPanel
    {
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

            RegisterCallbacks();
        }

        public void Unbind()
        {
            UnregisterCallbacks();
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

        private void OnPropTextChanged(ChangeEvent<string> evt) { }
        private void OnPropXChanged(ChangeEvent<float> evt) { }
        private void OnPropYChanged(ChangeEvent<float> evt) { }
        private void OnPropFontSizeChanged(ChangeEvent<int> evt) { }
        private void OnPropFontChanged(ChangeEvent<string> evt) { }
        private void OnPropEnabledChanged(ChangeEvent<bool> evt) { }

        private void OnPropColorChanged(ChangeEvent<string> evt)
        {
            UpdateColorSwatch(evt.newValue);
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

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MelonLoader;

namespace iOverlayer.UI
{
    public class EditorUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;

        // Top bar
        private Button _btnNew;
        private Button _btnOpen;
        private Button _btnSave;
        private Button _btnClose;
        private Label _lblFilename;

        // Toolbar
        private Button _toolText;
        private Button _toolSelect;

        // Property panel
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

        // Status bar
        private Label _selInfo;
        private Label _posInfo;
        private Label _countInfo;

        private enum Tool { Text, Select }

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            BindElements();
            RegisterCallbacks();
            SetActiveTool(_toolText);
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void BindElements()
        {
            // Top bar
            _btnNew = _root.Q<Button>("btn-new");
            _btnOpen = _root.Q<Button>("btn-open");
            _btnSave = _root.Q<Button>("btn-save");
            _btnClose = _root.Q<Button>("btn-close");
            _lblFilename = _root.Q<Label>("lbl-filename");

            // Toolbar
            _toolText = _root.Q<Button>("tool-text");
            _toolSelect = _root.Q<Button>("tool-select");

            // Property panel
            _noSelection = _root.Q<VisualElement>("no-selection");
            _propFields = _root.Q<VisualElement>("prop-fields");
            _propText = _root.Q<TextField>("prop-text");
            _propX = _root.Q<FloatField>("prop-x");
            _propY = _root.Q<FloatField>("prop-y");
            _propFontSize = _root.Q<IntegerField>("prop-fontSize");
            _propColor = _root.Q<TextField>("prop-color");
            _colorSwatch = _root.Q<VisualElement>("color-swatch");
            _propFont = _root.Q<DropdownField>("prop-font");
            _propEnabled = _root.Q<Toggle>("prop-enabled");

            // Status bar
            _selInfo = _root.Q<Label>("sel-info");
            _posInfo = _root.Q<Label>("pos-info");
            _countInfo = _root.Q<Label>("count-info");
        }

        private void RegisterCallbacks()
        {
            // Top bar buttons
            if (_btnNew != null)
                _btnNew.clicked += OnNewClicked;
            if (_btnOpen != null)
                _btnOpen.clicked += OnOpenClicked;
            if (_btnSave != null)
                _btnSave.clicked += OnSaveClicked;
            if (_btnClose != null)
                _btnClose.clicked += OnCloseClicked;

            // Tool buttons
            if (_toolText != null)
                _toolText.clicked += OnToolTextClicked;
            if (_toolSelect != null)
                _toolSelect.clicked += OnToolSelectClicked;

            // Property fields
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
            if (_btnNew != null)
                _btnNew.clicked -= OnNewClicked;
            if (_btnOpen != null)
                _btnOpen.clicked -= OnOpenClicked;
            if (_btnSave != null)
                _btnSave.clicked -= OnSaveClicked;
            if (_btnClose != null)
                _btnClose.clicked -= OnCloseClicked;

            if (_toolText != null)
                _toolText.clicked -= OnToolTextClicked;
            if (_toolSelect != null)
                _toolSelect.clicked -= OnToolSelectClicked;

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

        // ── Top bar handlers ──

        private void OnNewClicked()
        {
            MelonLogger.Msg("Editor: 新建 JSON");
        }

        private void OnOpenClicked()
        {
            MelonLogger.Msg("Editor: 打开 JSON");
        }

        private void OnSaveClicked()
        {
            MelonLogger.Msg("Editor: 保存");
        }

        private void OnCloseClicked()
        {
            var gameSceneName = MainUI.GameSceneName;
            AudioListener.pause = false;

            if (!string.IsNullOrEmpty(gameSceneName))
            {
                SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
            }
        }


        private void OnToolTextClicked()
        {
            SetActiveTool(_toolText);
            MelonLogger.Msg("Editor: 切换到文字工具");
        }

        private void OnToolSelectClicked()
        {
            SetActiveTool(_toolSelect);
            MelonLogger.Msg("Editor: 切换到选择工具");
        }

        private void SetActiveTool(Button active)
        {
            _toolText?.RemoveFromClassList("active");
            _toolSelect?.RemoveFromClassList("active");
            active?.AddToClassList("active");
        }

        // ── Property change handlers ──

        private void OnPropTextChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 文字内容 → {evt.newValue}");
        }

        private void OnPropXChanged(ChangeEvent<float> evt)
        {
            MelonLogger.Msg($"Editor: X → {evt.newValue}");
        }

        private void OnPropYChanged(ChangeEvent<float> evt)
        {
            MelonLogger.Msg($"Editor: Y → {evt.newValue}");
        }

        private void OnPropFontSizeChanged(ChangeEvent<int> evt)
        {
            MelonLogger.Msg($"Editor: 字号 → {evt.newValue}");
        }

        private void OnPropColorChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 颜色 → {evt.newValue}");
            UpdateColorSwatch(evt.newValue);
        }

        private void OnPropFontChanged(ChangeEvent<string> evt)
        {
            MelonLogger.Msg($"Editor: 字体 → {evt.newValue}");
        }

        private void OnPropEnabledChanged(ChangeEvent<bool> evt)
        {
            MelonLogger.Msg($"Editor: 显示 → {evt.newValue}");
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

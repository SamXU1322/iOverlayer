using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using iOverlayer.Config;

namespace iOverlayer.Editor
{
    public class EditorUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;
        private string _currentFilePath;
        private string _baseFileName;
        private bool _isDirty;

        private TopBar _topBar;
        private Toolbar _toolbar;
        private CanvasArea _canvasArea;
        private PropertyPanel _propertyPanel;
        private StatusBar _statusBar;
        private OverlayList _overlayList;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            _topBar = new TopBar();
            _topBar.Bind(_root);
            _topBar.SaveClicked += SaveToJson;
            _topBar.NewClicked += OnNewDocument;
            _topBar.OpenClicked += OpenFromJson;

            _canvasArea = new CanvasArea();
            _canvasArea.ContentChanged += MarkDirty;

            _toolbar = new Toolbar();
            _toolbar.Bind(_root);
            _toolbar.ToolChanged += tool => _canvasArea.CurrentTool = tool;
            _canvasArea.ToolChanged += tool => _toolbar.SetTool(tool);
            _canvasArea.SelectionChanged += OnSelectionChanged;

            _canvasArea.Bind(_root);

            _propertyPanel = new PropertyPanel();
            _propertyPanel.Bind(_root);
            _propertyPanel.ContentChanged += MarkDirty;

            _statusBar = new StatusBar();
            _statusBar.Bind(_root);

            _overlayList = new OverlayList();
            _overlayList.Bind(_root, _canvasArea);
        }

        private void MarkDirty()
        {
            if (_isDirty) return;
            _isDirty = true;
            UpdateFilenameDisplay();
        }

        private void ClearDirty()
        {
            _isDirty = false;
            UpdateFilenameDisplay();
        }

        private void UpdateFilenameDisplay()
        {
            _topBar.FilenameLabel.text = _isDirty ? _baseFileName + "*" : _baseFileName;
        }

        private void OnNewDocument()
        {
            _canvasArea.ClearAll();
            _currentFilePath = null;
            _baseFileName = "未保存";
            ClearDirty();
        }

        private void OpenFromJson()
        {
            ConfigManager.EnsureConfigDirectory();
            var filePath = FileDialog.ShowFilePicker("选择 JSON 文件", "*.json", ConfigManager.ConfigDirectory);
            if (string.IsNullOrEmpty(filePath)) return;

            try
            {
                var json = File.ReadAllText(filePath);
                var configFile = JsonConvert.DeserializeObject<OverlayConfigFile>(json);
                if (configFile == null) return;
                if (configFile.overlays == null)
                    configFile.overlays = new List<OverlayConfig>();

                _canvasArea.LoadFromConfigs(configFile.overlays);
                _currentFilePath = filePath;
                _baseFileName = Path.GetFileName(filePath);
                ClearDirty();
                MelonLoader.MelonLogger.Msg($"Opened: {filePath}");
            }
            catch (Exception ex)
            {
                MelonLoader.MelonLogger.Error($"Error opening file: {ex.Message}");
            }
        }

        private void OnSelectionChanged(VisualElement element)
        {
            if (element is Label label)
                _propertyPanel.SelectTarget(label);
            else
            {
                _propertyPanel.ClearTarget();
                _root.Focus();
            }
        }

        private void SaveToJson()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                ConfigManager.EnsureConfigDirectory();
                _currentFilePath = FileDialog.ShowFileSavePicker("保存叠加层配置", "*.json", "overlay.json", ConfigManager.ConfigDirectory);
                if (string.IsNullOrEmpty(_currentFilePath))
                    return;
            }

            var configs = _canvasArea.GetOverlayConfigs();
            var configFile = new OverlayConfigFile
            {
                version = "1.0",
                overlays = configs
            };
            var json = JsonConvert.SerializeObject(configFile, Formatting.Indented);
            File.WriteAllText(_currentFilePath, json);

            _baseFileName = Path.GetFileName(_currentFilePath);
            ClearDirty();
            MelonLoader.MelonLogger.Msg($"Saved: {configs.Count} overlays to {_currentFilePath}");
        }

        private void OnDisable()
        {
            if (_canvasArea != null) _canvasArea.ContentChanged -= MarkDirty;
            if (_propertyPanel != null) _propertyPanel.ContentChanged -= MarkDirty;
            _topBar?.Unbind();
            _toolbar?.Unbind();
            _canvasArea?.Unbind();
            _propertyPanel?.Unbind();
            _statusBar?.Unbind();
            _overlayList?.Unbind();
        }
    }
}

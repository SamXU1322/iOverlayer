using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MelonLoader;

namespace iOverlayer.UI
{
    public class MainUI : MonoBehaviour
    {
        private static string _gameSceneName;
        private static bool _editorSceneLoading;

        public static string GameSceneName => _gameSceneName;

        private UIDocument _uiDocument;
        private VisualElement _root;
        private Label _pageTitle;
        private Label _itemCount;
        private TextField _jsonTextField;
        private Button _jsonArrowBtn;
        private List<string> _jsonFiles;
        private volatile bool _filesLoaded;
        private List<string> _pendingFiles;
        private readonly object _fileLock = new object();
        private Button _refreshBtn;
        private Button _openEditorBtn;
        private Button _closeBtn;

        private VisualElement _jsonDetailPanel;
        private ScrollView _itemList;

        private List<Button> _navButtons;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            BindElements();
            RegisterCallbacks();

            if (_navButtons != null && _navButtons.Count > 0)
            {
                SelectNavTab(_navButtons[0]);
            }
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void BindElements()
        {
            _pageTitle = _root.Q<Label>(className: "page-title");
            _itemCount = _root.Q<Label>(className: "item-count");
            _jsonTextField = _root.Q<TextField>(className: "json-select-text");
            _jsonArrowBtn = _root.Q<Button>(className: "json-arrow-btn");
            _refreshBtn = _root.Q<Button>(className: "refresh-btn");
            _openEditorBtn = _root.Q<Button>(className: "open-editor-btn");
            _closeBtn = _root.Q<Button>(className: "close-btn");
            _jsonDetailPanel = _root.Q<VisualElement>(className: "json-dropdown-panel");
            _itemList = _root.Q<ScrollView>(className: "item-list");
            _navButtons = _root.Query<Button>(className: "nav-item").ToList();
        }

        private void RegisterCallbacks()
        {
            if (_jsonArrowBtn != null)
                _jsonArrowBtn.clicked += OnJsonArrowClicked;

            if (_closeBtn != null)
                _closeBtn.clicked += OnCloseClicked;

            if (_refreshBtn != null)
                _refreshBtn.clicked += OnRefreshClicked;

            if (_openEditorBtn != null)
                _openEditorBtn.clicked += OnOpenEditorClicked;

            if (_navButtons != null)
            {
                foreach (var btn in _navButtons)
                {
                    btn.clicked += () => SelectNavTab(btn);
                }
            }
        }

        private void UnregisterCallbacks()
        {
            if (_jsonArrowBtn != null)
                _jsonArrowBtn.clicked -= OnJsonArrowClicked;

            if (_closeBtn != null)
                _closeBtn.clicked -= OnCloseClicked;

            if (_refreshBtn != null)
                _refreshBtn.clicked -= OnRefreshClicked;

            if (_openEditorBtn != null)
                _openEditorBtn.clicked -= OnOpenEditorClicked;
        }

        private void SelectNavTab(Button selectedBtn)
        {
            foreach (var btn in _navButtons)
            {
                btn.RemoveFromClassList("active");
            }
            
            selectedBtn.AddToClassList("active");
            
            // 更新标题
            if (_pageTitle != null)
            {
                _pageTitle.text = selectedBtn.text;
            }

            RefreshContent(selectedBtn.text);
        }

        private void RefreshContent(string tabName)
        {
            
        }

        private VisualElement CreateDummyListItem(int index, string title)
        {
            var item = new VisualElement();
            item.AddToClassList("list-item");

            var indexLabel = new Label(index.ToString("D2"));
            indexLabel.AddToClassList("item-index");

            var infoContainer = new VisualElement();
            infoContainer.AddToClassList("item-info");
            
            var titleLabel = new Label(title);
            titleLabel.AddToClassList("item-title");
            
            var subTitleLabel = new Label("Description / details...");
            subTitleLabel.AddToClassList("item-subtitle");

            infoContainer.Add(titleLabel);
            infoContainer.Add(subTitleLabel);

            item.Add(indexLabel);
            item.Add(infoContainer);

            return item;
        }

        private bool _jsonPanelExpanded;

        private void OnJsonArrowClicked()
        {
            if (_jsonDetailPanel == null || _jsonArrowBtn == null) return;
            _jsonPanelExpanded = !_jsonPanelExpanded;

            if (_jsonPanelExpanded)
            {
                PositionDropdownPanel();
                _jsonDetailPanel.style.display = DisplayStyle.Flex;
                _jsonDetailPanel.schedule.Execute(() =>
                {
                    _jsonDetailPanel.AddToClassList("expanded");
                    _jsonArrowBtn.AddToClassList("expanded");
                }).StartingIn(16);
                _jsonArrowBtn.text = "▲";
                PopulateJsonDetailPanel();
            }
            else
            {
                _jsonDetailPanel.RemoveFromClassList("expanded");
                _jsonArrowBtn.RemoveFromClassList("expanded");
                _jsonArrowBtn.text = "▼";
                _jsonDetailPanel.schedule.Execute(() =>
                {
                    if (!_jsonPanelExpanded)
                        _jsonDetailPanel.style.display = DisplayStyle.None;
                }).StartingIn(210);
            }
        }

        private void PositionDropdownPanel()
        {
            if (_jsonTextField == null || _jsonDetailPanel == null) return;

            var selectWorld = _jsonTextField.parent.worldBound;
            var rootWorld = _root.worldBound;

            _jsonDetailPanel.style.left = selectWorld.x - rootWorld.x;
            _jsonDetailPanel.style.top = selectWorld.yMax - rootWorld.y + 4;
            _jsonDetailPanel.style.width = selectWorld.width;
        }

        private void PopulateJsonDetailPanel()
        {
            _jsonDetailPanel.Clear();

            _jsonFiles = Config.ConfigManager.GetJsonFiles();

            if (_jsonFiles == null || _jsonFiles.Count == 0)
            {
                var empty = new Label("No JSON files found");
                empty.AddToClassList("json-detail-empty");
                _jsonDetailPanel.Add(empty);
                return;
            }

            foreach (var file in _jsonFiles)
            {
                var item = new VisualElement();
                item.AddToClassList("json-file-item");
                item.userData = file;

                var icon = new Label("{}");
                icon.AddToClassList("json-file-icon");

                var name = new Label(file);
                name.AddToClassList("json-file-name");

                item.Add(icon);
                item.Add(name);

                var isSelected = file == (_jsonTextField?.value ?? "");
                if (isSelected)
                    item.AddToClassList("json-file-item--selected");

                item.RegisterCallback<ClickEvent>(OnJsonFileClicked);

                _jsonDetailPanel.Add(item);
            }
        }

        private void OnJsonFileClicked(ClickEvent evt)
        {
            if (evt.target is VisualElement target && target.userData is string fileName)
            {
                if (_jsonTextField != null)
                    _jsonTextField.value = fileName;

                CollapseJsonPanel();
            }
        }

        private void CollapseJsonPanel()
        {
            _jsonPanelExpanded = false;
            _jsonDetailPanel.RemoveFromClassList("expanded");
            _jsonArrowBtn.RemoveFromClassList("expanded");
            _jsonArrowBtn.text = "▼";
            _jsonDetailPanel.schedule.Execute(() =>
            {
                if (!_jsonPanelExpanded)
                    _jsonDetailPanel.style.display = DisplayStyle.None;
            }).StartingIn(210);
        }

        private void Update()
        {
            if (!_filesLoaded) return;

            List<string> files;
            lock (_fileLock)
            {
                files = _pendingFiles;
                _pendingFiles = null;
                _filesLoaded = false;
            }

            _jsonFiles = files ?? new List<string>();

            if (_jsonFiles.Count > 0 && _jsonTextField != null)
                _jsonTextField.value = _jsonFiles[0];
            else if (_jsonTextField != null)
                _jsonTextField.value = "";
        }

        private void OnRefreshClicked()
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var files = Config.ConfigManager.GetJsonFiles();
                lock (_fileLock)
                {
                    _pendingFiles = files;
                    _filesLoaded = true;
                }
            });
        }

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnOpenEditorClicked()
        {
            MelonLogger.Msg("Open Editor Button Clicked!");
            _gameSceneName = SceneManager.GetActiveScene().name;

            AudioListener.pause = true;

            _editorSceneLoading = true;
            BundleLoader.LoadScene("editorscenes", "EditorScenes", LoadSceneMode.Single);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!_editorSceneLoading || scene.name != "EditorScenes") return;
            _editorSceneLoading = false;

            // 找到场景中带 UIDocument 的 GameObject 并附加 EditorUI
            foreach (var rootGo in scene.GetRootGameObjects())
            {
                var uiDoc = rootGo.GetComponentInChildren<UIDocument>();
                if (uiDoc != null)
                {
                    uiDoc.gameObject.AddComponent<EditorUI>();
                    return;
                }
            }

            MelonLogger.Warning("EditorScenes 中未找到 UIDocument，无法附加 EditorUI");
        }
    }
}
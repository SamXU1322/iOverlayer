using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using MelonLoader;

namespace iOverlayer.UI
{
    public class MainUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;
        private Label _pageTitle;
        private Label _itemCount;
        private DropdownField _jsonDropdown;
        private Button _openEditorBtn;
        private Button _closeBtn;

        private ScrollView _itemList;

        private List<Button> _navButtons;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            BindElements();
            RegisterCallbacks();

            // 初始化默认选中第一个标签
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
            _jsonDropdown = _root.Q<DropdownField>(className: "json-dropdown");
            _openEditorBtn = _root.Q<Button>(className: "open-editor-btn");
            _closeBtn = _root.Q<Button>(className: "close-btn");
            _itemList = _root.Q<ScrollView>(className: "item-list");
            _navButtons = _root.Query<Button>(className: "nav-item").ToList();
        }

        private void RegisterCallbacks()
        {
            if (_closeBtn != null)
                _closeBtn.clicked += OnCloseClicked;

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
            if (_closeBtn != null)
                _closeBtn.clicked -= OnCloseClicked;

            if (_openEditorBtn != null)
                _openEditorBtn.clicked -= OnOpenEditorClicked;
        }

        private void SelectNavTab(Button selectedBtn)
        {
            // 移除所有按钮的 active 状态
            foreach (var btn in _navButtons)
            {
                btn.RemoveFromClassList("active");
            }
            
            // 激活当前按钮
            selectedBtn.AddToClassList("active");
            
            // 更新标题
            if (_pageTitle != null)
            {
                _pageTitle.text = selectedBtn.text;
            }

            // 根据不同的标签页加载内容
            RefreshContent(selectedBtn.text);
        }

        private void RefreshContent(string tabName)
        {
            if (_itemList == null) return;
            
            _itemList.Clear();
            int dummyCount = 5;
            for (int i = 0; i < dummyCount; i++)
            {
                _itemList.Add(CreateDummyListItem(i + 1, $"{tabName} Item {i+1}"));
            }

            if (_itemCount != null)
            {
                _itemCount.text = $"{dummyCount} Items";
            }
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

        private void OnCloseClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnOpenEditorClicked()
        {
            MelonLogger.Msg("Open Editor Button Clicked!");
        }
    }
}
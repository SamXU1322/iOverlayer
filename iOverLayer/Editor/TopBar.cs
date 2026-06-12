using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MelonLoader;
using iOverlayer.Config;
using iOverlayer.UI;

namespace iOverlayer.Editor
{
    public class TopBar
    {
        public event Action SaveClicked;
        public event Action NewClicked;
        public event Action OpenClicked;
        private Button _btnNew;
        private Button _btnOpen;
        private Button _btnSave;
        private Button _btnClose;
        private Label _lblFilename;

        public Label FilenameLabel => _lblFilename;

        public void Bind(VisualElement root)
        {
            _btnNew = root.Q<Button>("btn-new");
            _btnOpen = root.Q<Button>("btn-open");
            _btnSave = root.Q<Button>("btn-save");
            _btnClose = root.Q<Button>("btn-close");
            _lblFilename = root.Q<Label>("lbl-filename");

            RegisterCallbacks();
        }

        public void Unbind()
        {
            UnregisterCallbacks();
            _btnNew = null;
            _btnOpen = null;
            _btnSave = null;
            _btnClose = null;
            _lblFilename = null;
        }

        private void RegisterCallbacks()
        {
            if (_btnNew != null) _btnNew.clicked += OnNewClicked;
            if (_btnOpen != null) _btnOpen.clicked += OnOpenClicked;
            if (_btnSave != null) _btnSave.clicked += OnSaveClicked;
            if (_btnClose != null) _btnClose.clicked += OnCloseClicked;
        }

        private void UnregisterCallbacks()
        {
            if (_btnNew != null) _btnNew.clicked -= OnNewClicked;
            if (_btnOpen != null) _btnOpen.clicked -= OnOpenClicked;
            if (_btnSave != null) _btnSave.clicked -= OnSaveClicked;
            if (_btnClose != null) _btnClose.clicked -= OnCloseClicked;
        }

        private void OnNewClicked()
        {
            NewClicked?.Invoke();
        }

        private void OnOpenClicked()
        {
            OpenClicked?.Invoke();
        }

        private void OnSaveClicked()
        {
            SaveClicked?.Invoke();
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
    }
}

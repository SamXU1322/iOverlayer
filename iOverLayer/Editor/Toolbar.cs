using System;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class Toolbar
    {
        public event Action<EditorTool> ToolChanged;

        private Button _toolText;
        private Button _toolSelect;
        private EditorTool _currentTool;

        private Action _onTextClicked;
        private Action _onSelectClicked;

        public EditorTool CurrentTool => _currentTool;

        public void Bind(VisualElement root)
        {
            _toolText = root.Q<Button>("tool-text");
            _toolSelect = root.Q<Button>("tool-select");
            RegisterCallbacks();
            SetTool(EditorTool.Select);
        }

        public void Unbind()
        {
            UnregisterCallbacks();
            _toolText = null;
            _toolSelect = null;
        }

        private void RegisterCallbacks()
        {
            _onTextClicked = () => SetTool(EditorTool.Text);
            _onSelectClicked = () => SetTool(EditorTool.Select);

            if (_toolText != null) _toolText.clicked += _onTextClicked;
            if (_toolSelect != null) _toolSelect.clicked += _onSelectClicked;
        }
        private void UnregisterCallbacks()
        {
            if (_toolText != null) _toolText.clicked -= _onTextClicked;
            if (_toolSelect != null) _toolSelect.clicked -= _onSelectClicked;

            _onTextClicked = null;
            _onSelectClicked = null;
        }

        private void SetTool(EditorTool tool)
        {
            _currentTool = tool;

            _toolText?.EnableInClassList("active", tool == EditorTool.Text);
            _toolSelect?.EnableInClassList("active", tool == EditorTool.Select);

            ToolChanged?.Invoke(tool);
        }
    }
}

using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class Toolbar
    {
        private Button _toolText;
        private Button _toolSelect;

        public void Bind(VisualElement root)
        {
            _toolText = root.Q<Button>("tool-text");
            _toolSelect = root.Q<Button>("tool-select");

            if (_toolText != null) _toolText.clicked += OnToolTextClicked;
            if (_toolSelect != null) _toolSelect.clicked += OnToolSelectClicked;

            SetActive(_toolText);
        }

        public void Unbind()
        {
            if (_toolText != null) _toolText.clicked -= OnToolTextClicked;
            if (_toolSelect != null) _toolSelect.clicked -= OnToolSelectClicked;
            _toolText = null;
            _toolSelect = null;
        }

        private void OnToolTextClicked()
        {
            SetActive(_toolText);
        }

        private void OnToolSelectClicked()
        {
            SetActive(_toolSelect);
        }

        private void SetActive(Button active)
        {
            _toolText?.RemoveFromClassList("active");
            _toolSelect?.RemoveFromClassList("active");
            active?.AddToClassList("active");
        }
    }
}

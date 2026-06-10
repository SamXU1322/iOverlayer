using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class EditorUI : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _root;

        private TopBar _topBar;
        private Toolbar _toolbar;
        private CanvasArea _canvasArea;
        private PropertyPanel _propertyPanel;
        private StatusBar _statusBar;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            if (_root == null) return;

            _topBar = new TopBar();
            _topBar.Bind(_root);

            _canvasArea = new CanvasArea();

            _toolbar = new Toolbar();
            _toolbar.Bind(_root);
            _toolbar.ToolChanged += tool => _canvasArea.CurrentTool = tool;

            _canvasArea.Bind(_root);

            _propertyPanel = new PropertyPanel();
            _propertyPanel.Bind(_root);

            _statusBar = new StatusBar();
            _statusBar.Bind(_root);
        }

        private void OnDisable()
        {
            _topBar?.Unbind();
            _toolbar?.Unbind();
            _canvasArea?.Unbind();
            _propertyPanel?.Unbind();
            _statusBar?.Unbind();
        }
    }
}

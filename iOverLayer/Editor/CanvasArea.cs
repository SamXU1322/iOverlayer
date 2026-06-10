using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class CanvasArea
    {
        private VisualElement _canvasWrap;
        private VisualElement _canvas;

        public VisualElement Wrap => _canvasWrap;
        public VisualElement Canvas => _canvas;

        public void Bind(VisualElement root)
        {
            _canvasWrap = root.Q<VisualElement>("canvas-wrap");
            _canvas = root.Q<VisualElement>("canvas");
        }

        public void Unbind()
        {
            _canvasWrap = null;
            _canvas = null;
        }
    }
}

using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class CanvasArea
    {
        private VisualElement _canvasWrap;
        private VisualElement _canvas;

        public VisualElement Wrap => _canvasWrap;
        public VisualElement Canvas => _canvas;
        public EditorTool CurrentTool { get; set; }

        public void Bind(VisualElement root)
        {
            _canvasWrap = root.Q<VisualElement>("canvas-wrap");
            _canvas = root.Q<VisualElement>("canvas");

            RegisterCallbacks();
        }

        public void Unbind()
        {
            UnregisterCallbacks();
            _canvasWrap = null;
            _canvas = null;
        }

        private void RegisterCallbacks()
        {
            _canvas.RegisterCallback<ClickEvent>(OnCanvasClick);
            _canvas.RegisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _canvas.RegisterCallback<PointerUpEvent>(OnCanvasPointerUp);
        }

        private void UnregisterCallbacks()
        {
            _canvas.UnregisterCallback<ClickEvent>(OnCanvasClick);
            _canvas.UnregisterCallback<PointerMoveEvent>(OnCanvasPointerMove);
            _canvas.UnregisterCallback<PointerUpEvent>(OnCanvasPointerUp);
        }

        private void OnCanvasClick(ClickEvent evt)
        {
            switch (CurrentTool)
            {
                case EditorTool.Text:
                    // 文字工具：在点击处新建文字
                    break;
                case EditorTool.Select:
                    // 选择工具：选中/取消选中
                    break;
            }
        }

        private void OnCanvasPointerMove(PointerMoveEvent evt)
        {
        }

        private void OnCanvasPointerUp(PointerUpEvent evt)
        {
        }
    }
}

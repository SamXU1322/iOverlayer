using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class StatusBar
    {
        public event System.Action<int, int> CanvasSizeChanged;

        private Label _selInfo;
        private Label _posInfo;
        private Label _countInfo;
        private Button _btnCanvasProps;
        private VisualElement _canvasPropsPopup;
        private IntegerField _canvasWidthField;
        private IntegerField _canvasHeightField;
        private Button _canvasApply;
        private Button _canvasClose;
        private int _canvasWidth = 1920;
        private int _canvasHeight = 1080;

        public Label SelInfo => _selInfo;
        public Label PosInfo => _posInfo;
        public Label CountInfo => _countInfo;

        public void Bind(VisualElement root)
        {
            _selInfo = root.Q<Label>("sel-info");
            _posInfo = root.Q<Label>("pos-info");
            _countInfo = root.Q<Label>("count-info");
            _btnCanvasProps = root.Q<Button>("btn-canvas-props");
            _canvasPropsPopup = root.Q<VisualElement>("canvas-props-popup");
            _canvasWidthField = root.Q<IntegerField>("canvas-width");
            _canvasHeightField = root.Q<IntegerField>("canvas-height");
            _canvasApply = root.Q<Button>("canvas-apply");
            _canvasClose = root.Q<Button>("canvas-props-close");

            if (_btnCanvasProps != null)
                _btnCanvasProps.clicked += OnCanvasPropsClicked;
            if (_canvasApply != null)
                _canvasApply.clicked += OnCanvasApplyClicked;
            if (_canvasClose != null)
                _canvasClose.clicked += HideCanvasProps;
        }

        public void Unbind()
        {
            if (_btnCanvasProps != null)
                _btnCanvasProps.clicked -= OnCanvasPropsClicked;
            if (_canvasApply != null)
                _canvasApply.clicked -= OnCanvasApplyClicked;
            if (_canvasClose != null)
                _canvasClose.clicked -= HideCanvasProps;

            _selInfo = null;
            _posInfo = null;
            _countInfo = null;
            _btnCanvasProps = null;
            _canvasPropsPopup = null;
            _canvasWidthField = null;
            _canvasHeightField = null;
            _canvasApply = null;
            _canvasClose = null;
        }

        public void SetCanvasSize(int width, int height)
        {
            _canvasWidth = width;
            _canvasHeight = height;
            if (_btnCanvasProps != null)
                _btnCanvasProps.text = $"画布: {width}×{height}";
        }

        private void OnCanvasPropsClicked()
        {
            if (_canvasPropsPopup == null) return;
            if (_canvasPropsPopup.style.display != DisplayStyle.None)
            {
                HideCanvasProps();
                return;
            }
            if (_canvasWidthField != null)
                _canvasWidthField.SetValueWithoutNotify(_canvasWidth);
            if (_canvasHeightField != null)
                _canvasHeightField.SetValueWithoutNotify(_canvasHeight);
            _canvasPropsPopup.style.display = DisplayStyle.Flex;
        }

        private void OnCanvasApplyClicked()
        {
            var width = _canvasWidthField != null ? Mathf.Max(1, _canvasWidthField.value) : _canvasWidth;
            var height = _canvasHeightField != null ? Mathf.Max(1, _canvasHeightField.value) : _canvasHeight;
            HideCanvasProps();
            SetCanvasSize(width, height);
            CanvasSizeChanged?.Invoke(width, height);
        }

        private void HideCanvasProps()
        {
            if (_canvasPropsPopup != null)
                _canvasPropsPopup.style.display = DisplayStyle.None;
        }
    }
}

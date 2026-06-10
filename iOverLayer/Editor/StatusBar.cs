using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    public class StatusBar
    {
        private Label _selInfo;
        private Label _posInfo;
        private Label _countInfo;

        public Label SelInfo => _selInfo;
        public Label PosInfo => _posInfo;
        public Label CountInfo => _countInfo;

        public void Bind(VisualElement root)
        {
            _selInfo = root.Q<Label>("sel-info");
            _posInfo = root.Q<Label>("pos-info");
            _countInfo = root.Q<Label>("count-info");
        }

        public void Unbind()
        {
            _selInfo = null;
            _posInfo = null;
            _countInfo = null;
        }
    }
}

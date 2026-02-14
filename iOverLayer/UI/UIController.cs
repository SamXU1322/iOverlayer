using iOverLayer;
using UnityEngine;
using UnityEngine.UIElements;
namespace iOverLayer.UI
{ 
    public class UIController: MonoBehaviour
    {
        private VisualElement _root;
        public VisualElement Root => _root;

        private KeyCode _appearKey = KeyCode.F1;
        void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _root.style.display = DisplayStyle.None;
            ButtonBox.ApplyExitButton(_root);
        }

        private void AppearUI(KeyCode key)
        {
            if(Input.GetKeyDown(key) && _root.style.display == DisplayStyle.Flex)
            {
                LogSystem.Info($"{key} Pressed");
                _root.style.display = DisplayStyle.Flex;
            }
        }
        public void Update()
        {
            AppearUI(_appearKey);
        }
        public void SetAppearKey(KeyCode key)
        {
            _appearKey = key;
        }
    }
}
    
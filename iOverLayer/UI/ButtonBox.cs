using UnityEngine;
using UnityEngine.UIElements;
namespace iOverLayer.UI
{
    public static class ButtonBox
    {
        private static Button ExitButton;
        public static void ApplyExitButton(VisualElement root)
        {
            ExitButton = root.Q<Button>("ExitButton");
            ExitButton.clicked += () => OnExitButtonClicked(root);
        }
        private static void OnExitButtonClicked(VisualElement root)
        {
            LogSystem.Info("Exit button clicked");
            root.style.display = DisplayStyle.None;
        }
    }
}


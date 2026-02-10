using UnityEngine;
using UnityEngine.UIElements;

public class UIController: MonoBehaviour
{
    private VisualElement root;
    private Button exitButton;
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        root.focusable = true;
        root.Focus();
        exitButton = root.Q<Button>("ExitButton");
        exitButton.clicked += OnExitButtonClicked;
    }
    private void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        root.style.display = DisplayStyle.None;
    }
    private void OnF1()
    {
        Debug.Log("F1 pressed on root");
        root.style.display = DisplayStyle.Flex;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && root.style.display == DisplayStyle.None)
        {
            OnF1();
        }
    }
}
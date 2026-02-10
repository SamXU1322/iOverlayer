using iOverLayer.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class UIController: MonoBehaviour
{
    private VisualElement _root;
    public VisualElement Root => _root;

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _root.style.display = DisplayStyle.None;
        ButtonBox.ApplyExitButton(_root);
    }

    private void OnF1()
    {
        Debug.Log("F1 pressed on root");
        _root.style.display = DisplayStyle.Flex;
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1) && _root.style.display == DisplayStyle.None)
        {
            OnF1();
        }
    }
}
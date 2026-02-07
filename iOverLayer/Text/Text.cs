using TMPro;
using UnityEngine;
namespace iOverLayer.Text
{
    public class Text: MonoBehaviour
    {
        private int _id;
        private bool _isSelected = false;
        private TextMeshProUGUI _textMesh;
        public int ID => _id;
        public void Awake()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>(true);

            if (_textMesh == null)
            {
                LogSystem.Error("TextMeshProUGUI component not found on Text GameObject.");
                return;
            }
            TextDefaultSetting.SetDefaultText(ref _textMesh);
        }
        public void setId(int id)
        {
            _id = id;
        }
        public void setText(string text)
        {
            _textMesh.text = text;
        }
        public void setColor(Color color)
        {
            _textMesh.color = color;
        }
    }
}
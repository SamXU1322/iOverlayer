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
        }
        public void setId(int id)
        {
            _id = id;
        }
        
    }
}
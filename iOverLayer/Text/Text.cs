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
        public void setId(int id)
        {
            _id = id;
        }
        
    }
}
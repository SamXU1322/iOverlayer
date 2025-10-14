using iOverlayer.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;

namespace iOverlayer.GUI
{
    public class iOverlayerGUI: MonoBehaviour
    {
        public RectTransform rectTransform;
        public Transform Transform = Main.GUIObject.transform;
        public GameObject Name;
        public Transform iOverlayerTransform;
        public TextMeshProUGUI[] NameText { get; private set; }
        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            SetupDefaultProperties();
            iOverlayerTransform = transform.Find("Navigation/Viewport/Content/iOverlayer");
            if (iOverlayerTransform!= null)
            {
                NameText = iOverlayerTransform.GetComponentsInChildren<TextMeshProUGUI>();
                foreach (var text in NameText)
                {
                    text.font = Main.BoldFontAsset;
                }
            }
        }
        private void SetupDefaultProperties()
        { 
            if(rectTransform != null)
            {
                rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
            }
        }
    }
}
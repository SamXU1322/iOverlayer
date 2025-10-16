using System;
using iOverlayer.Core;
using iOverlayer.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;
using UnityModManagerNet;

namespace iOverlayer.GUI
{
    public class iOverlayerGUI: MonoBehaviour
    {
        public UnityModManager.ModEntry.ModLogger Logger = Main.Logger;
        public RectTransform rectTransform;
        public Transform Transform = ModManager.GUIObject.transform;
        public GameObject Name;
        public Transform iOverlayerTransform;
        public static bool isGUIVisible = false;
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

            isGUIVisible = ModManager.GUIObject.activeSelf;
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
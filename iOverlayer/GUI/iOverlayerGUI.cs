using System;
using System.Reflection;
using iOverlayer.Core;
using iOverlayer.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using UnityModManagerNet;

namespace iOverlayer.GUI
{
    public class iOverlayerGUI: MonoBehaviour
    {
        public UnityModManager.ModEntry.ModLogger Logger = Main.Logger;
        public RectTransform rectTransform;
        public Transform Transform = ModManager.GUIObject.transform;
        public GameObject Name;
        public static bool isGUIVisible = false;
        public static Shader sr_msdf = (Shader)typeof(ShaderUtilities).GetProperty("ShaderRef_MobileSDF", (BindingFlags)15420).GetValue(null);
        public TextMeshProUGUI[] NameText { get; private set; }
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            SetupDefaultProperties();
            if(Transform != null)
            {
                NameText = Transform.GetComponentsInChildren<TextMeshProUGUI>();
                Image[] images = Transform.GetComponentsInChildren<Image>();
                foreach (TextMeshProUGUI text in NameText)
                {
                    text.fontSharedMaterial.shader = sr_msdf;
                }
            }
            else
            {
                Main.Logger.Log("Error: iOverlayer Transform not found");
            }
            ModManager.GUIObject.SetActive(false);
            isGUIVisible = false;
        }
        private void SetupDefaultProperties()
        { 
            if(rectTransform != null)
            {
                rectTransform.localScale = new Vector3(0.5f, 0.5f, 1);
                
            }
        }
        public void ToggleGUIVisibility()
        {
            isGUIVisible = !isGUIVisible;
            this.gameObject.SetActive(isGUIVisible);
        }
    }
}
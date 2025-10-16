using System;
using iOverlayer.GUI;
using iOverlayer.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace iOverlayer.Core
{
    public class ModManager:MonoBehaviour
    {
        public static GameObject GUIObject;
        
        private void Awake()
        {
            InitializedPublicCanvas();
            GUIObject = CreateUIObject();
            iOverlayerGUI iOverlayerGUI =  GUIObject.AddComponent<iOverlayerGUI>();
            iOverlayerGUI.Initialize();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleGUIVisibility();
                Main.Logger.Log("F1 Pressed");
            }
        }
        public void ToggleGUIVisibility()
        {
            iOverlayerGUI.isGUIVisible = !iOverlayerGUI.isGUIVisible;
            GUIObject.SetActive(iOverlayerGUI.isGUIVisible);
        }
        public void InitializedPublicCanvas()
        {
            Canvas mainCanvas = this.gameObject.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            mainCanvas.sortingOrder = 10001;
            CanvasScaler scaler = this.gameObject.AddComponent<CanvasScaler>(); 
            scaler.referenceResolution = new Vector2(3840, 2160);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; 
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.referencePixelsPerUnit = 0.5f;
            this.gameObject.AddComponent<GraphicRaycaster>();
        }
        private GameObject CreateUIObject()
        {
            string assetBundlePath = System.IO.Path.Combine(Main.ModEntry.Path, "ioverlayergui");
            AssetBundle ba = AssetBundle.LoadFromFile(assetBundlePath);
            if (ba != null)
            {
                GameObject GUIPrefab = ba.LoadAsset<GameObject>("iOverlayerGUI");
                ba.Unload(false);
                if (GUIPrefab != null)
                {
                    return Instantiate(GUIPrefab, this.transform);
                }
                else
                {
                    Main.Logger.Log("Failed to load 'text' prefab from AssetBundle");
                }
            }
            else
            {
                Main.Logger.Log("Failed to load AssetBundle from path: " + assetBundlePath);
            }
            return null;
        }
    }
}
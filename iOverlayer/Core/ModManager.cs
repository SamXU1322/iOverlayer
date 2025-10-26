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
        public static iOverlayerGUI iOverlayerGUI;
        public static GameObject[] TextSet;
        public static TextBehavior[] TextBehaviors;
        public int TextCount { get; set; } = 0;

        private void Awake()
        {
            InitializedPublicCanvas();
            GUIObject = CreateUIObject();
            iOverlayerGUI = GUIObject.AddComponent<iOverlayerGUI>();
            CreateTextObject();
            TextBehaviors[0].text.text = "{TotalTime}"
            ;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                iOverlayerGUI.ToggleGUIVisibility();
                Main.Logger.Log("F1 Pressed");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                foreach (TextBehavior behavior in TextBehaviors)
                {
                    behavior.ChangeVisibility();
                }
                Main.Logger.Log("F2 Pressed");
            }
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
            InitializedEventSystem();
        }

        private void InitializedEventSystem()
        {
            if (UnityEngine.EventSystems.EventSystem.current == null)
            {
                GameObject eventSystemObject = new GameObject("EventSystem");
                eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                DontDestroyOnLoad(eventSystemObject);
            }
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
                    GameObject TempGUIObject = GameObject.Instantiate(GUIPrefab, this.gameObject.transform);
                    return TempGUIObject;
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
        private GameObject CreateTextObject()
        {
            string assetBundlePath = System.IO.Path.Combine(Main.ModEntry.Path, "text");
            AssetBundle ba = AssetBundle.LoadFromFile(assetBundlePath);
            if (ba != null)
            {
                GameObject textPrefab = ba.LoadAsset<GameObject>("text");
                ba.Unload(false);
                if (textPrefab != null)
                {
                    GameObject textObject = GameObject.Instantiate(textPrefab, this.gameObject.transform);
                    textObject.name = $"Text{TextCount}";
                    TextBehavior textBehavior = textObject.AddComponent<TextBehavior>();
                    if (TextSet == null)
                    {
                        TextSet = new GameObject[1];
                        TextSet[0] = textObject;
                    }
                    else
                    {
                        Array.Resize(ref TextSet, TextSet.Length + 1);
                        TextSet[TextSet.Length - 1] = textObject;
                    }

                    if (TextBehaviors == null)
                    {
                        TextBehaviors = new TextBehavior[1];
                        TextBehaviors[0] = textBehavior;
                    }
                    else
                    {
                        Array.Resize(ref TextBehaviors, TextBehaviors.Length + 1);
                        TextBehaviors[TextBehaviors.Length - 1] = textBehavior;
                    }
                    return textObject;
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
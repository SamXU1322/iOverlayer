using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using iOverlayer.Config;
using iOverlayer.Script;
using UnityEngine.UI;

namespace iOverlayer.Text
{
    public class OverlayRenderer : MonoBehaviour
    {
        private const string EditorSceneName = "EditorScenes";

        private Canvas _canvas;
        private CanvasScaler _scaler;
        private RectTransform _root;
        private readonly List<GameObject> _texts = new List<GameObject>();
        private int _canvasWidth = 1920;
        private int _canvasHeight = 1080;
        private readonly ScriptHost _scripts = new ScriptHost();

        private void Awake()
        {
            BuildCanvas();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (_canvas != null)
                Destroy(_canvas.gameObject);
        }

        private void Start()
        {
            Reload();
            StartCoroutine(FixLayout());
        }

        private void Update()
        {
            _scripts.Update(Time.deltaTime);
        }

        private void BuildCanvas()
        {
            var go = new GameObject("iOverlayer_OverlayCanvas");
            Object.DontDestroyOnLoad(go);
            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 90;

            _scaler = go.AddComponent<CanvasScaler>();
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = new Vector2(_canvasWidth, _canvasHeight);
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            _root = go.transform as RectTransform;
            if (_root == null)
                _root = go.AddComponent<RectTransform>();
        }

        private System.Collections.IEnumerator FixLayout()
        {
            yield return null;
            if (_root.rect.width <= 100f && _root.rect.height <= 100f)
                _root.sizeDelta = new Vector2(_canvasWidth, _canvasHeight);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _canvas.gameObject.SetActive(scene.name != EditorSceneName);
            if (scene.name != EditorSceneName)
                Reload();
        }

        private void Reload()
        {
            LoadConfig("overlay.json");
        }

        private void LoadConfig(string fileName)
        {
            Clear();
            _scripts.Clear();
            if (string.IsNullOrEmpty(fileName)) return;

            var configFile = ConfigManager.LoadConfig(fileName);
            if (configFile == null || configFile.overlays == null) return;

            _canvasWidth = configFile.canvasWidth > 0 ? configFile.canvasWidth : 1920;
            _canvasHeight = configFile.canvasHeight > 0 ? configFile.canvasHeight : 1080;
            if (_scaler != null)
                _scaler.referenceResolution = new Vector2(_canvasWidth, _canvasHeight);

            foreach (var overlay in configFile.overlays)
            {
                if (overlay == null) continue;
                if (string.IsNullOrEmpty(overlay.text)) continue;
                if (!overlay.enabled || overlay.hidden) continue;
                SpawnText(overlay);
            }
        }

        private void SpawnText(OverlayConfig overlay)
        {
            var go = new GameObject("Overlay_" + overlay.name);
            go.transform.SetParent(_root, false);
            _texts.Add(go);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = overlay.text;
            tmp.raycastTarget = false;
            tmp.textWrappingMode = overlay.width > 0
                ? TextWrappingModes.Normal
                : TextWrappingModes.NoWrap;

            var font = OverlayFonts.GetOrCreate(overlay.font, overlay.fontPath);
            if (font != null)
                tmp.font = font;

            tmp.fontSize = Mathf.Max(1, overlay.fontSize);
            tmp.color = ParseColor(overlay.color);
            tmp.alignment = overlay.width > 0
                ? MapAlignment(overlay.textAlign)
                : TextAlignmentOptions.TopLeft;

            var rect = tmp.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(overlay.x, -overlay.y);
            rect.sizeDelta = new Vector2(overlay.width > 0 ? overlay.width : 0f, 0f);

            if (!string.IsNullOrEmpty(overlay.script))
            {
                var script = ScriptCompiler.CreateInstance(overlay.script);
                if (script != null)
                    _scripts.Attach(go, script, new OverlayScriptContext(overlay, tmp, go));
            }
        }

        private static Color ParseColor(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out var c) ? c : Color.white;
        }

        private static TextAlignmentOptions MapAlignment(string textAlign)
        {
            if (!string.IsNullOrEmpty(textAlign) && System.Enum.TryParse(textAlign, out TextAnchor anchor))
            {
                switch (anchor)
                {
                    case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
                    case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
                    case TextAnchor.MiddleLeft: return TextAlignmentOptions.Left;
                    case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
                    case TextAnchor.MiddleRight: return TextAlignmentOptions.Right;
                    case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
                    case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
                    case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
                    default: return TextAlignmentOptions.TopLeft;
                }
            }
            return TextAlignmentOptions.TopLeft;
        }

        private void Clear()
        {
            foreach (var go in _texts)
                if (go != null) Destroy(go);
            _texts.Clear();
        }
    }
}

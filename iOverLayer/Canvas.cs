using UnityEngine;
using UnityEngine.UI;

namespace iOverLayer
{
    internal static class Canvas
    {
        private static GameObject _root;
        private static UnityEngine.Canvas _canvas;

        public static UnityEngine.Canvas Instance => _canvas;
        public static GameObject Root => _root;

        public static void Init()
        {
            if (_root != null) return;

            _root = new GameObject("iOverLayer");
            Object.DontDestroyOnLoad(_root);

            _canvas = _root.AddComponent<UnityEngine.Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 30000;

            var scaler = _root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            LogSystem.Info("Canvas initialized.");
        }
    }
}
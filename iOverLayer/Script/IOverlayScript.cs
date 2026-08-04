using TMPro;
using UnityEngine;
using iOverlayer.Config;

namespace iOverlayer.Script
{
    public interface IOverlayScript
    {
        void OnInit(OverlayScriptContext ctx);
        void OnUpdate(OverlayScriptContext ctx, float deltaTime);
        void OnDestroy();
    }

    public class OverlayScriptContext
    {
        public OverlayConfig Config { get; }
        public TextMeshProUGUI Text { get; }
        public GameObject GameObject { get; }

        public OverlayScriptContext(OverlayConfig config, TextMeshProUGUI text, GameObject gameObject)
        {
            Config = config;
            Text = text;
            GameObject = gameObject;
        }

        public void SetText(string text) => Text.text = text;

        public void SetPosition(Vector2 designPos)
        {
            var rect = Text.rectTransform;
            rect.anchoredPosition = new Vector2(designPos.x, -designPos.y);
        }

        public void SetFontSize(float size) => Text.fontSize = size;

        public void SetColor(Color color) => Text.color = color;

        public void SetAlpha(float alpha)
        {
            var c = Text.color;
            c.a = Mathf.Clamp01(alpha);
            Text.color = c;
        }

        public void SetVisible(bool visible) => Text.gameObject.SetActive(visible);
    }
}

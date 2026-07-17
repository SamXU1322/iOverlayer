using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    internal static class SvgIconLoader
    {
        private static readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

        public static Texture2D Load(string name)
        {
            if (_cache.TryGetValue(name, out var cached)) return cached;
            var assembly = Assembly.GetExecutingAssembly();
            var resName = "iOverlayer." + name + ".svg";
            using var stream = assembly.GetManifestResourceStream(resName);
            if (stream == null) return null;
            using var reader = new StreamReader(stream);
            var svg = reader.ReadToEnd();
            var tex = Render(svg);
            tex.name = name;
            _cache[name] = tex;
            return tex;
        }

        public static void ApplyToButton(Button btn, string iconName, Color tintColor)
        {
            if (btn == null) return;
            var tex = Load(iconName);
            if (tex == null) return;
            btn.text = "";
            btn.style.backgroundImage = new StyleBackground(tex);
            btn.style.backgroundSize = new BackgroundSize(16, 16);
            btn.style.unityBackgroundImageTintColor = new StyleColor(tintColor);
        }

        private static Texture2D Render(string svg)
        {
            const int size = 32;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var clear = new Color[size * size];
            for (int i = 0; i < clear.Length; i++) clear[i] = Color.clear;
            tex.SetPixels(clear);

            var doc = new XmlDocument();
            doc.LoadXml(svg);
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("svg", "http://www.w3.org/2000/svg");

            float vbW = 16f, vbH = 16f;
            var svgRoot = doc.SelectSingleNode("//svg:svg", nsmgr);
            if (svgRoot != null)
            {
                var vb = svgRoot.Attributes?["viewBox"]?.Value;
                if (!string.IsNullOrEmpty(vb))
                {
                    var parts = vb.Split(' ');
                    if (parts.Length == 4) { float.TryParse(parts[2], out vbW); float.TryParse(parts[3], out vbH); }
                }
            }

            foreach (XmlNode child in doc.DocumentElement.ChildNodes)
            {
                if (child.LocalName == "rect") DrawRect(tex, child, vbW, vbH, size);
                else if (child.LocalName == "circle") DrawCircle(tex, child, vbW, vbH, size);
                else if (child.LocalName == "ellipse") DrawEllipse(tex, child, vbW, vbH, size);
                else if (child.LocalName == "path") DrawPathStroke(tex, child, vbW, vbH, size, svgRoot);
            }

            tex.Apply();
            return tex;
        }

        private static void DrawRect(Texture2D tex, XmlNode node, float vbW, float vbH, int size)
        {
            float x = Parse(node, "x"), y = Parse(node, "y");
            float w = Parse(node, "width"), h = Parse(node, "height");
            bool filled = GetFill(node) != Color.clear;
            if (!filled) return;

            int x0 = R(x / vbW * size), y0 = R((1f - (y + h) / vbH) * size);
            int x1 = R((x + w) / vbW * size), y1 = R((1f - y / vbH) * size);
            FillRect(tex, x0, y0, x1, y1, size, filled ? GetFill(node) : Color.white);
        }

        private static void DrawCircle(Texture2D tex, XmlNode node, float vbW, float vbH, int size)
        {
            float cx = Parse(node, "cx"), cy = Parse(node, "cy"), r = Parse(node, "r");
            var color = GetFill(node);
            bool isFilled = color != Color.clear;
            bool hasStroke = GetStroke(node) != Color.clear;
            if (!isFilled && !hasStroke) return;

            float sx = size / vbW, sy = size / vbH;
            int px = R(cx * sx), py = R((1f - cy / vbH) * size);
            int ri = R(r * Mathf.Min(sx, sy));

            if (isFilled)
            {
                for (int dy = -ri; dy <= ri; dy++)
                    for (int dx = -ri; dx <= ri; dx++)
                        if (dx * dx + dy * dy <= ri * ri)
                            SetPx(tex, px + dx, py + dy, size, color);
            }
            else
            {
                int sw = R(ParseAttr(node, "stroke-width", 1f) * Mathf.Min(sx, sy));
                DrawCircleStroke(tex, px, py, ri, size, GetStroke(node), sw);
            }
        }

        private static void DrawEllipse(Texture2D tex, XmlNode node, float vbW, float vbH, int size)
        {
            float cx = Parse(node, "cx"), cy = Parse(node, "cy");
            float rx = Parse(node, "rx"), ry = Parse(node, "ry");
            var color = GetFill(node);
            bool isFilled = color != Color.clear;
            bool hasStroke = GetStroke(node) != Color.clear;
            if (!isFilled && !hasStroke) return;

            float sx = size / vbW, sy = size / vbH;
            int pcx = R(cx * sx), pcy = R((1f - cy / vbH) * size);
            int irx = R(rx * sx), iry = R(ry * sy);

            if (isFilled)
            {
                for (int dy = -iry; dy <= iry; dy++)
                    for (int dx = -irx; dx <= irx; dx++)
                        if (irx > 0 && iry > 0 && (float)dx * dx / (irx * irx) + (float)dy * dy / (iry * iry) <= 1f)
                            SetPx(tex, pcx + dx, pcy + dy, size, color);
            }
            else
            {
                int sw = R(ParseAttr(node, "stroke-width", 1.3f) * Mathf.Min(sx, sy));
                var sc = GetStroke(node);
                for (int angle = 0; angle < 360; angle++)
                {
                    float rad = angle * Mathf.Deg2Rad;
                    int ex = R(pcx + irx * Mathf.Cos(rad));
                    int ey = R(pcy + iry * Mathf.Sin(rad));
                    for (int s = -sw / 2; s <= sw / 2; s++)
                        SetPx(tex, ex, ey + s, size, sc);
                }
            }
        }

        private static void DrawPathStroke(Texture2D tex, XmlNode node, float vbW, float vbH, int size, XmlNode svgRoot)
        {
            var stroke = GetStroke(node);
            if (stroke == Color.clear) return;
            var d = node.Attributes?["d"]?.Value;
            if (string.IsNullOrEmpty(d)) return;

            float sw = ParseAttr(node, "stroke-width", 1.5f);
            var points = ParsePath(d);
            if (points.Count < 2) return;

            float sx = size / vbW, sy = size / vbH;
            int halfSw = R(sw * Mathf.Min(sx, sy) * 0.5f);

            for (int i = 1; i < points.Count; i++)
            {
                int x0 = R(points[i - 1].x * sx), y0 = R((1f - points[i - 1].y / vbH) * size);
                int x1 = R(points[i].x * sx), y1 = R((1f - points[i].y / vbH) * size);
                DrawLine(tex, x0, y0, x1, y1, size, stroke, halfSw);
            }
        }

        private static List<Vector2> ParsePath(string d)
        {
            var pts = new List<Vector2>();
            var tokens = System.Text.RegularExpressions.Regex.Matches(d, @"-?\d+\.?\d*");
            for (int i = 0; i + 1 < tokens.Count; i += 2)
            {
                if (float.TryParse(tokens[i].Value, out float x) && float.TryParse(tokens[i + 1].Value, out float y))
                    pts.Add(new Vector2(x, y));
            }
            return pts;
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, int size, Color c, int halfSw)
        {
            int dx = Mathf.Abs(x1 - x0), dy = Mathf.Abs(y1 - y0);
            int steps = Mathf.Max(dx, dy);
            if (steps == 0) { SetPx(tex, x0, y0, size, c); return; }
            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                int px = R(Mathf.Lerp(x0, x1, t)), py = R(Mathf.Lerp(y0, y1, t));
                for (int s = -halfSw; s <= halfSw; s++)
                {
                    SetPx(tex, px + s, py, size, c);
                    SetPx(tex, px, py + s, size, c);
                }
            }
        }

        private static void DrawCircleStroke(Texture2D tex, int cx, int cy, int r, int size, Color c, int sw)
        {
            int halfSw = sw / 2;
            for (int angle = 0; angle < 360; angle++)
            {
                float rad = angle * Mathf.Deg2Rad;
                int ex = R(cx + r * Mathf.Cos(rad)), ey = R(cy + r * Mathf.Sin(rad));
                for (int s = -halfSw; s <= halfSw; s++)
                    SetPx(tex, ex, ey + s, size, c);
            }
        }

        private static void FillRect(Texture2D tex, int x0, int y0, int x1, int y1, int size, Color c)
        {
            for (int y = y0; y < y1; y++)
                for (int x = x0; x < x1; x++)
                    SetPx(tex, x, y, size, c);
        }

        private static Color GetFill(XmlNode node)
        {
            var v = node.Attributes?["fill"]?.Value;
            if (string.IsNullOrEmpty(v) || v == "none") return Color.clear;
            return Color.white;
        }

        private static Color GetStroke(XmlNode node)
        {
            var v = node.Attributes?["stroke"]?.Value;
            if (string.IsNullOrEmpty(v) || v == "none") return Color.clear;
            return Color.white;
        }

        private static float Parse(XmlNode node, string attr) => ParseAttr(node, attr, 0f);

        private static float ParseAttr(XmlNode node, string attr, float def)
        {
            var a = node.Attributes?[attr]?.Value;
            return string.IsNullOrEmpty(a) ? def : float.TryParse(a, out float v) ? v : def;
        }

        private static int R(float v) => Mathf.RoundToInt(v);

        private static void SetPx(Texture2D tex, int x, int y, int size, Color c)
        {
            if (x >= 0 && x < size && y >= 0 && y < size)
                tex.SetPixel(x, y, c);
        }
    }
}

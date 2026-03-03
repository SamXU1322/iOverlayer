using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace iOverLayer.Text
{
    public static class TextManager
    {
        
        private static Dictionary<string,Text> _texts = new Dictionary<string, Text>();
        private static int _textCount = 0;
        public static Dictionary<string, Text> Texts => _texts;
        
        public static void Init(string JsonPath)
        {
            TextFont.LoadFontAsset("Maplestory Bold SDF");
            LoadFromJsonFile(JsonPath);
        }
        public static bool LoadFromJsonFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                LogSystem.Error("JSON file path is empty.");
                return false;
            }

            if (!File.Exists(filePath))
            {
                LogSystem.Error($"JSON file not found: {filePath}");
                return false;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                LogSystem.Info($"Loading JSON from: {filePath}");
                LogSystem.Info($"JSON length: {json.Length}");
                return LoadFromJson(json);
            }
            catch (Exception ex)
            {
                LogSystem.Error($"Read JSON file failed: {ex.Message}");
                return false;
            }
        }
        public static bool LoadFromJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                LogSystem.Error("JSON content is empty.");
                return false;
            }

            try
            {
                TextConfigRoot root = JsonConvert.DeserializeObject<TextConfigRoot>(json);
                if (root == null || root.Texts == null || root.Texts.Length == 0)
                {
                    LogSystem.Error("JSON format invalid or no text items found.");
                    return false;
                }

                bool allSuccess = true;

                foreach (TextConfigItem item in root.Texts)
                {
                    if (item == null || string.IsNullOrEmpty(item.Name))
                    {
                        allSuccess = false;
                        continue;
                    }

                    if (!Create(item))
                    {
                        allSuccess = false;
                        continue;
                    }

                    if (item.Color != null && item.Color.Length >= 4)
                    {
                        SetColor(item.Name, new Color(item.Color[0], item.Color[1], item.Color[2], item.Color[3]));
                    }

                    if (!string.IsNullOrEmpty(item.FontName))
                    {
                        SetFont(item.Name, item.FontName);
                    }

                    if (item.Position != null && item.Position.Length >= 2 && _texts.ContainsKey(item.Name))
                    {
                        _texts[item.Name].TextMesh.rectTransform.anchoredPosition =
                            new Vector2(item.Position[0], item.Position[1]);
                    }
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                LogSystem.Error($"Parse JSON failed: {ex.Message}");
                return false;
            }
        }
        public static bool ExportTextsToJson(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                LogSystem.Error("Export path is empty.");
                return false;
            }

            try
            {
                var items = new List<TextConfigItem>();
                int index = 0;

                foreach (var pair in _texts)
                {
                    if (pair.Value == null || pair.Value.TextMesh == null)
                    {
                        continue;
                    }

                    var mesh = pair.Value.TextMesh;
                    var color = mesh.color;
                    var rect = mesh.rectTransform;
                    Vector2 pos = rect != null ? rect.anchoredPosition : Vector2.zero;

                    items.Add(new TextConfigItem
                    {
                        ID = ++index,
                        Name = pair.Key,
                        Information = mesh.text,
                        FontName = mesh.font != null ? mesh.font.name : "Default",
                        Color = new float[] { color.r, color.g, color.b, color.a },
                        Position = new float[] { pos.x, pos.y },
                        FontSize = (int)mesh.fontSize,
                        FontType = "Asset"
                    });
                }

                TextConfigRoot root = new TextConfigRoot
                {
                    Texts = items.ToArray()
                };

                string dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                string json = JsonConvert.SerializeObject(root, Formatting.Indented);
                File.WriteAllText(filePath, json);

                LogSystem.Info($"Exported {_texts.Count} texts to JSON: {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                LogSystem.Error($"Export texts to JSON failed: {ex.Message}");
                return false;
            }
        public static bool Create(TextConfigItem textConfigItem)
        {
            
            if (Canvas.Root == null)
            {
                LogSystem.Error("Canvas.Root is null. Call Canvas.Init() first.");
                return false;
            }

            GameObject prefab = AssetLoader.LoadPrefabAssetBundle("textprefab", "Text");
            if (prefab == null)
            {
                LogSystem.Error("Text prefab load failed.");
                return false;
            }
            if(textConfigItem.Name != "")
            {
                GameObject instance = UnityEngine.Object.Instantiate(prefab, Canvas.Root.transform);
                Text text = instance.AddComponent<Text>();
                text.SetText(textConfigItem.Information);
                _textCount++;
                textConfigItem.ID = _textCount;
                _texts[textConfigItem.Name] = text;
                text.UnShow();
                LogSystem.Info($"{textConfigItem.Name} created.");
                return true;
            }
            else
            {
                LogSystem.Error("Text name cannot be empty.");
                return false;
            }
            
        }
        public static void Delete(string TextName)
        {
            if (_texts.ContainsKey(TextName))
            {
                _texts.Remove(TextName);
            }
            else
            {
                LogSystem.Error($"Text with ID {TextName} does not exist.");
            }
        }
        public static void SetText(string TextName, string text)
        {
            _texts[TextName].SetText(text);
        }
        public static void SetColor(string TextName, Color color)
        {
            _texts[TextName].SetColor(color);
        }
        public static void SetFont(string TextName, string FontName)
        {
            _texts[TextName].SetFont(FontName);
        }
    }
}
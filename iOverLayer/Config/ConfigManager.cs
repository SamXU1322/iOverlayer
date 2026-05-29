using System;
using System.Collections.Generic;
using System.IO;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;

namespace iOverlayer.Config
{
    public static class ConfigManager
    {
        private static readonly string _configDir = Path.Combine(MelonEnvironment.UserDataDirectory, "iOverlayer");

        public static string ConfigDirectory => _configDir;

        public static List<string> GetJsonFiles()
        {
            var files = new List<string>();
            if (!Directory.Exists(_configDir))
                return files;

            foreach (var file in Directory.GetFiles(_configDir, "*.json"))
            {
                files.Add(Path.GetFileName(file));
            }
            return files;
        }

        public static OverlayConfigFile LoadConfig(string fileName)
        {
            var filePath = Path.Combine(_configDir, fileName);
            if (!File.Exists(filePath))
            {
                MelonLogger.Warning($"Config file not found: {filePath}");
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var configFile = JsonUtility.FromJson<OverlayConfigFile>(json);
                if (configFile == null)
                {
                    MelonLogger.Warning($"Failed to parse JSON: {fileName}");
                    return null;
                }
                if (configFile.overlays == null)
                    configFile.overlays = new List<OverlayConfig>();
                return configFile;
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error loading config '{fileName}': {ex.Message}");
                return null;
            }
        }

        public static void SaveConfig(string fileName, OverlayConfigFile configFile)
        {
            if (!Directory.Exists(_configDir))
                Directory.CreateDirectory(_configDir);

            var filePath = Path.Combine(_configDir, fileName);
            try
            {
                var json = JsonUtility.ToJson(configFile, prettyPrint: true);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Error saving config '{fileName}': {ex.Message}");
            }
        }

        public static void EnsureConfigDirectory()
        {
            if (!Directory.Exists(_configDir))
                Directory.CreateDirectory(_configDir);
        }
    }
}

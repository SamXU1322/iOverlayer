using System;
using System.IO;
using UnityModManagerNet;
namespace iOverLayer
{
    public static class LogSystem

    {
        private static string _logFilePath = "iOverLayer.log";
        private static UnityModManager.ModEntry _modEntry;
        public static void Init(UnityModManager.ModEntry modEntry)
        {
            _modEntry = modEntry;
            _logFilePath = Path.Combine(_modEntry.Path, "iOverLayer.log");
        }
        public static void Info(string message)
        {
            WriteLine("INFO", message);
        }
        public static void Warning(string message)
        {
            WriteLine("WARNING", message);
        }
        public static void Error(string message)
        {
            WriteLine("ERROR", message);
        }
        private static void WriteLine(string level,string message)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            File.AppendAllText(_logFilePath, line + Environment.NewLine);
        }
    }
}

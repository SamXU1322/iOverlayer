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
            Info("LogSystem initialized.");
        }
        public static void Info(string message)
        {
        #if DEBUG
            _modEntry.Logger.Log(message);
        #endif
        }
        public static void Warning(string message)
        {
        #if DEBUG
            _modEntry.Logger.Warning(message);
        #endif
        }
        public static void Error(string message)
        {
            _modEntry.Logger.Error(message);
        }
        
    }
}

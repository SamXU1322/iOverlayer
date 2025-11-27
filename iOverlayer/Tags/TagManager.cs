using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using iOverlayer.Tags.Patch;

namespace iOverlayer.Tags
{
    public static class TagManager
    {
        private static Dictionary<string, Func<string, string>> tagHandlers =
            new Dictionary<string, Func<string, string>>();

        static TagManager()
        {
            RegisterTagHandler(@"\{TileBPM:(\d+)\}", HandleTileBPM);
        }

        public static void RegisterTagHandler(string pattern, Func<string, string> handler)
        {
            tagHandlers[pattern] = handler;
        }

        public static string ProcessTags(string inputText)
        {
            string result = inputText;
            foreach (var kvp in tagHandlers)
            {
                result = Regex.Replace(result, kvp.Key,match => kvp.Value(match.Value));
            }
            return result;
        }

        private static string HandleTileBPM(string input)
        {
            var match = Regex.Match(input, @"\{TileBPM:(\d+)\}");
            int decimalPlaces = int.TryParse(match.Groups[1].Value, out int parsedValue) ? parsedValue : 3;
            return BPMPatch.Bpm.Tilebpm.ToString("F" + decimalPlaces);
        }
    }
}
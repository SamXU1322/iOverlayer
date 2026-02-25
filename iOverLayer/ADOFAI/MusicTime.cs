using System;
using iOverLayer.Text;
using UnityEngine;

namespace iOverLayer.ADOFAI
{
    internal static class MusicTime
    {
        private static bool _isPrinted = false;
        public static int CurMinute;
        public static int CurSecond;
        public static int CurMilliSecond;
        public static int TotalMinute;
        public static int TotalSecond;
        public static int TotalMilliSecond;
        public static void Reset()
        {
            CurMinute = CurSecond = CurMilliSecond = 0;
            TotalMinute = TotalSecond = TotalMilliSecond = 0;
        }
        public static void OnUpdateMusic(scrConductor __instance)
        {
            if (scrController.instance.paused || !__instance.isGameWorld) return;
            AudioSource song = __instance.song;
            if (!song.clip) return;
            TimeSpan nowt = TimeSpan.FromSeconds(song.time);
            TimeSpan tott = TimeSpan.FromSeconds(song.clip.length);
            CurMinute = nowt.Minutes;
            CurSecond = nowt.Seconds;
            CurMilliSecond = nowt.Milliseconds;
            TotalMinute = tott.Minutes;
            TotalSecond = tott.Seconds;
            TotalMilliSecond = tott.Milliseconds;
            if (TextManager.Texts.ContainsKey("MusicTime"))
            {
                TextManager.Texts["MusicTime"].SetText($"Music Time {CurMinute}:{CurSecond}~{TotalMinute}:{TotalSecond}");
            }
            else
            {
                if (_isPrinted) return;
                _isPrinted = true;
                LogSystem.Warning("MusicTime Tag is not existed.");
            }
        }
    }
}

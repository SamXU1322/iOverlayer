using System;
using HarmonyLib;
using UnityEngine;
using iOverlayer.Core;


namespace iOverlayer.Tags.Patches
{
    public static class SongPatch
    {
        public static Song Song = new Song();
        [HarmonyPatch(typeof(scrConductor), "Update")]
        public static class scrConductor_Update_Patch
        {
            public static void Postfix(scrConductor __instance)
            {
                if (!__instance.isGameWorld) return;
                AudioSource song =__instance.song;
                if(!song.clip) return;
                TimeSpan nowTime = TimeSpan.FromSeconds(song.time);
                TimeSpan totalTime = TimeSpan.FromSeconds(song.clip.length);
                Song.CurMinute = nowTime.Minutes;
                Song.CurSecond = nowTime.Seconds;
                Song.CurMillisecond = nowTime.Milliseconds;
                Song.TotalMinute = totalTime.Minutes;
                Song.TotalSecond = totalTime.Seconds;
                Song.TotalMillisecond = totalTime.Milliseconds;
            }
        }
    }
}
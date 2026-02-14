using iOverLayer.Patch;
using System;
using UnityEngine;
namespace iOverLayer.ADOFAI
{
    public static class BPM
    {
        private static float lastTileBPM = -1;
        private static float lastCurBPM = -1;
        public static void UpdateBPM()
        {
            scrFloor floor = scrController.instance.currFloor ?? scrController.instance.firstFloor;
            scrConductor conductor = scrConductor.instance;
            float bpm = (float)(conductor.bpm * conductor.song.pitch * scrController.instance.speed);
            float cbpm = floor.nextfloor ? (float)(60.0 / (floor.nextfloor.entryTime - floor.entryTime) * conductor.song.pitch) : bpm;
            float kps = cbpm / 60;
            LogSystem.Info (" BPM: " + bpm + " CBPM: " + cbpm + " KPS: " + kps);
            if (lastTileBPM == bpm && lastCurBPM == cbpm) return;
            lastTileBPM = bpm;
            lastCurBPM = cbpm;
        }
    }
}

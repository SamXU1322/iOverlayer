using System;
using HarmonyLib;
using iOverlayer.Core;

namespace iOverlayer.Tags.Patch
{
    internal static class BPMPatch
    {
        private static BPM _bpm = new BPM();
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        internal class scrPlanet_MoveToNextFloor_Patch
        {
            public static void Postfix(scrPlanet __instance,scrFloor floor)
            {
                _bpm.bpm = __instance.conductor.bpm;
                if(!__instance.controller.gameworld) return;
                if (floor.nextfloor == null) return;
                bool isTwirl = scrController.instance.isCW;
                double val = scrMisc.GetAngleMoved(floor.entryangle,floor.exitangle,isTwirl)/3.1415927410125732*180;
                double angle =Math.Round(val);
                if(angle==0) angle=360;
                double speed = 0;
                speed = __instance.controller.speed;
                ModManager.TextBehaviors[0].text.text=$"CurBPM:{Math.Round(_bpm.bpm*speed*180/angle,3)}";
            }
        }
    }
    
}
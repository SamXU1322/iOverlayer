using System;
using HarmonyLib;
using iOverlayer.Core;
using iOverlayer.Text;

namespace iOverlayer.Tags.Patch
{
    public static class BPMPatch
    {
        public static BPM Bpm = new BPM();
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        public class scrPlanet_MoveToNextFloor_Patch
        {
            public static void Postfix(scrPlanet __instance,scrFloor floor)
            {
                Bpm.bpm = __instance.conductor.bpm;
                if(!__instance.controller.gameworld) return;
                if (floor.nextfloor == null) return;
                bool isTwirl = scrController.instance.isCW;
                double val = scrMisc.GetAngleMoved(floor.entryangle,floor.exitangle,isTwirl)/3.1415927410125732*180;
                double angle =Math.Round(val);
                if(angle==0) angle=360;
                double speed = 0;
                speed = __instance.controller.speed;
                Bpm.Tilebpm = Bpm.bpm * speed;
                if (ModManager.TextBehaviors != null)
                {
                    foreach(var behavior in ModManager.TextBehaviors)
                    {
                        if(behavior.isVisible)
                        {
                            behavior.text.text = TagManager.ProcessTags(behavior.text.text);
                            behavior.text.ForceMeshUpdate();
                        }
                    }
                }
            }
        }
    }
    
}
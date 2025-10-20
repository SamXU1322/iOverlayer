using HarmonyLib;
using UnityEngine;

namespace iOverlayer.Core
{
    internal static class Adofai
    {
        
        [HarmonyPatch(typeof(scrCalibrationPlanet), "Start")]
        internal static class scrCalibrationPlanet_Start
        {
            private static void Postfix()
            {
                var textObject = GameObject.Find("iOverlayer/text(Clone)");
                if (textObject != null)
                {
                    textObject.SetActive(false);
                }
            }
        }
        [HarmonyPatch(typeof(scrPlanet),"MoveToNextFloor")]
        internal static class scrPlanet_MoveToNextFloor
        { 
            public static void Postfix(scrPlanet __instance,scrFloor floor)
            {
                if (__instance.controller.gameworld) return;
                if (floor.nextfloor == null) return;
                bool isTwirl = scrController.instance.isCW;
                double val = scrMisc.GetAngleMoved(floor.entryangle, floor.exitangle, isTwirl);
                ModManager.TextBehaviors[0].text.text = val.ToString();
            }
        }
    }
}
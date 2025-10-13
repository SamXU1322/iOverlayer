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
    }
}
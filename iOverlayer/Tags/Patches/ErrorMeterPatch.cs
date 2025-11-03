using HarmonyLib;
using UnityEngine;

namespace iOverlayer.Tags.Patches
{
    public class ErrorMeterPatch
    {
        private static void SetErrorMeterSize(float size = 1f)
        {
            var controller = scrController.instance;
            var errorMeter = controller.errorMeter;
            if (errorMeter && controller.gameworld && errorMeter.gameObject.activeSelf)
            {
                errorMeter.meterScale = size;
                errorMeter.wrapperRectTransform.localScale = new Vector3(size, size, size);
				
            }
        }
        private static void SetErrorMeterHide(bool isHide = true)
        {
            var controller = scrController.instance;
            var errorMeter = controller.errorMeter;
            if (errorMeter && controller.gameworld && errorMeter.gameObject.activeSelf)
            {
                errorMeter.gameObject.SetActive(isHide);
            }
        }
        [HarmonyPatch(typeof(scrController), "paused", MethodType.Setter)]
        private static class SrcControllerChangeHitErrorMeterPatch
        {
            public static void Postfix() => SetErrorMeterSize();
        }
        [HarmonyPatch(typeof(scrPlanet), "MoveToNextFloor")]
        private static class ScrPlanetChangeHitErrorMeterPatch
        {
            public static void Postfix() => SetErrorMeterSize();
        }

        [HarmonyPatch(typeof(TaroCutsceneScript), "DisplayText")]
        private static class TaroCutsceneScriptChangeHitErrorMeterPatch
        {
            public static void Postfix() => SetErrorMeterSize();
        }
    }
    
}
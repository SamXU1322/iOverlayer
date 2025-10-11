using HarmonyLib;
namespace iOverlayer
{
    internal static class Adofai
    {
        private static float bpm = 0, pitch = 0, playbackSpeed = 1;
        private static bool fitrst = true, beforedt = false;
        private static double beforebpm = 0;

        [HarmonyPatch(typeof(scrCalibrationPlanet), "Start")]
        internal static class scrCalibrationPlanet_Start
        {
            private static void Postfix()
            {
                Main.TextGUI.textObject.SetActive(false);
            }
        }
        
    }
}
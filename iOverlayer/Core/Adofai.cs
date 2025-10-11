using HarmonyLib;
namespace iOverlayer.Core
{
    internal static class Adofai
    {
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
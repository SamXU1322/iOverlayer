// using HarmonyLib;
// using UnityEngine;
//
// namespace iOverlayer.Core
// {
//     internal static class Adofai
//     {
//         [HarmonyPatch(typeof(scrCalibrationPlanet), "Start")]
//         internal static class scrCalibrationPlanet_Start
//         {
//             private static void Postfix()
//             {
//                 Main.TextGUI.textObject.SetActive(false);
//             }
//         }
//         [HarmonyPatch(typeof(scrCalibrationPlanet), "Update")]
//         internal static class scrCalibrationPlanet_Update
//         { 
//             private static void Postfix()
//             {
//                 if (Input.GetKeyDown(KeyCode.F1))
//                 {
//                     Main.TextGUI.textObject.SetActive(!Main.TextGUI.textObject.activeSelf);
//                 }
//             }
//         }
//         
//     }
// }
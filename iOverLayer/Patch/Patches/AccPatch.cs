using iOverLayer.ADOFAI;
using System.Reflection;
namespace iOverLayer.Patch.Patches
{
    internal static class AccPatch
    {
        public static void AddPatch()
        {
            PatchManager.AddPatch(typeof(scrMistakesManager), "CalculatePercentAcc", typeof(AccPatch).GetMethod("OnAccuracyChange",BindingFlags.NonPublic | BindingFlags.Static), PatchType.Postfix, false);
        }
        
        [iOverLayerPatch(typeof(scrMistakesManager), "CalculatePercentAcc", PatchType.Postfix, false)]
        private static void OnAccuracyChange()
        {
           Accuracy.UpdateAccuracy();
        }
    }
}

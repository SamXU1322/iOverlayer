using iOverLayer.ADOFAI;
namespace iOverLayer.Patch.Patches
{
    internal static class ProgressPatch
    {
        [iOverLayerPatch(typeof(scrPlanet), "MoveToNextFloor", PatchType.Postfix, false)]
        private static void OnProgressChange()
        {
            Progress.UpdateProgress();
        }
    }
}

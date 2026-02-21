using iOverLayer.ADOFAI;

namespace iOverLayer.Patch.Patches
{
    internal class TimingScalePatch
    {
        [iOverLayerPatch(typeof(scrPlanet), "MoveToNextFloor", PatchType.Postfix, true)]
        private static void OnMoveToNextFloor()
        {
            TimingScale.UpdateTimingScale();
        }
    }
}

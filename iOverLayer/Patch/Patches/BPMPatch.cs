using iOverLayer.ADOFAI;
namespace iOverLayer.Patch.Patches
{
    internal static class BPMPatch
    {
        [iOverLayerPatch(typeof(scrController),"Hit",PatchType.Postfix,true)]
        private static void OnHit()
        {
            BPM.UpdateBPM();
        }
    }
}

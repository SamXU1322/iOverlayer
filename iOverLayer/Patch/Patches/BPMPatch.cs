using iOverLayer.ADOFAI;
namespace iOverLayer.Patch.Patches
{
    internal class BPMPatch
    {
        [iOverLayerPatch(typeof(scrController),"Hit",PatchType.Postfix,true)]
        private static void OnHit()
        {
            BPM.UpdateBPM();
        }
    }
}

using iOverLayer.ADOFAI;
using System.Runtime;
using static UnityEngine.UI.CanvasScaler;

namespace iOverLayer.Patch.Patches
{
    internal class ComBoPatch
    {
        public static int combo = 0;
        public static void AddPatch()
        {
            PatchManager.AddPatch(typeof(scrMistakesManager), "AddHit", typeof(ComBoPatch).GetMethod("OnHit"), PatchType.Postfix, true);

        }
        [iOverLayerPatch(typeof(scrMistakesManager), "AddHit", PatchType.Postfix, true)]
        public static void OnHit(HitMargin hit)
        { 
            if (hit == HitMargin.Perfect || hit == HitMargin.Auto) ComBo.UpdateComBo(++combo);
            else if (hit != HitMargin.Auto) ComBo.UpdateComBo(combo = 0);
            LogSystem.Info($"{scrMistakesManager.hitMarginsCount[3] + scrMistakesManager.hitMarginsCount[10]}");
        }
    }
}

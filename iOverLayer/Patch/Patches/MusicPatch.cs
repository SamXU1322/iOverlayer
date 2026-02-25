using iOverLayer.ADOFAI;
using iOverLayer.Text;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace iOverLayer.Patch.Patches
{
    internal static class MusicPatch
    {
        [iOverLayerPatch(typeof(scrConductor),"Update",PatchType.Postfix,true)]
        public static void OnUpdate()
        {
            MusicTime.OnUpdateMusic(scrConductor.instance);
        }
    }
}

using iOverLayer.ADOFAI;
namespace iOverLayer.Patch.Patches
{
    internal static class TilePatch
    {
        [iOverLayerPatch(typeof(scnGame), "Update", PatchType.Postfix, false)]
        public static void OnUpdateTile()
        {
            Tile.UpdateTile();
        }
    }
}

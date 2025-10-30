using HarmonyLib;

namespace iOverlayer.Tags.Patches
{
    public static class UIPatches
    { 
        [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
        public static class test
        {
            public static void Postfix()
            {
                var uiController = scrController.instance;
                uiController.txtLevelName.gameObject.SetActive(false);
            }
        }
    }
}
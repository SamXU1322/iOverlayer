using TMPro;
using UnityEngine;
namespace iOverLayer.Text
{
    public static class TextDefaultSetting
    {
        public static Color DefaultColor = Color.white;
        public static int DefaultFontSize = 36;
        public static TMP_FontAsset DefaultAssetFont = null;
        public static TMP_FontAsset DefaultOSFont = null;
        public static void SetDefaultText(ref TextMeshProUGUI _textMesh,string type)
        {
            _textMesh.color = DefaultColor;
            _textMesh.fontSize = DefaultFontSize;
            TextFont.LoadFontAsset("Maplestory Bold SDF");
            if(type == "Asset")
            {
                _textMesh.font = TextFont.FontAsset["Maplestory Bold SDF"];
            }
            else if (type == "OS")
            {
                TextFont.CreateOSAsset("");
                _textMesh.font = TextFont.FontAsset[""];
            }
        }
    }
}

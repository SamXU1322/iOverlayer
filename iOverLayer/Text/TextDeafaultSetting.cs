using TMPro;
using UnityEngine;
namespace iOverLayer.Text
{
    public static class TextDefaultSetting
    {
        public static Color DefaultColor = Color.white;
        public static int DefaultFontSize = 36;
        public static TMP_FontAsset DefaultFontAsset = null;
        public static void SetDefaultText(ref TextMeshProUGUI _textMesh)
        {
            _textMesh.color = DefaultColor;
            _textMesh.fontSize = DefaultFontSize;
        }
    }
}

using UnityEngine;
using UnityEngine.UIElements;

namespace iOverlayer.Editor
{
    // Per-label metadata stored in Label.userData.
    public class LabelData
    {
        public string name;
        public string font = "Arial";
        public bool locked;
        public TextAnchor textAlign = TextAnchor.UpperLeft;

        public static LabelData Of(Label label)
        {
            if (label.userData is LabelData data) return data;
            data = new LabelData();
            label.userData = data;
            return data;
        }
    }
}

using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.LowLevel;
namespace iOverlayer.Text
{
    internal class TextBehavior : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public RectTransform rectTransform;

        public void Initialize()
        {
            rectTransform = GetComponent<RectTransform>();
            text = GetComponentInChildren<TextMeshProUGUI>();
            SetupDefaultProperties();
        }

        private void SetupDefaultProperties()
        {
            if (text != null)
            {
                text.text = "Hello, iOverlayer!";
                text.alignment = TextAlignmentOptions.Center;
                text.fontSize = 16;
                text.color = Color.white;
                text.overflowMode = TextOverflowModes.Overflow;
                text.enableWordWrapping = false;
                text.ForceMeshUpdate();
            }

            if (rectTransform != null)
            {
                Vector2 pos = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = pos;
                rectTransform.anchorMax = pos;
                rectTransform.pivot = pos;
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }
    }
}
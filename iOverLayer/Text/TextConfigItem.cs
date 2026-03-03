using System;
using System.Collections.Generic;
using TMPro;

namespace iOverLayer.Text
{
    [Serializable]
    public class TextConfigItem
    {
        public int ID;
        public string Name;
        public string Information;
        public string FontName;
        public float[] Color;
        public float[] Position;
        public int FontSize;
        public string FontType;
        public TextConfigItem() {
            ID = -1;
            Name = "";
            Information = "";
            FontName = "";
            Color = new float[] { 1f, 1f, 1f, 1f };
            Position = new float[] { 0f, 0f };
            FontSize = 36;
            FontType = "Asset";
        }
        public TextConfigItem(int id,string name, string information, string fontName, float[] color, float[] position, int fontSize, string fontType)
        {
            ID = id;
            Name = name;
            Information = information;
            FontName = fontName;
            Color = color;
            Position = position;
            FontSize = fontSize;
            FontType = fontType;
        }
        
    }
    
}

using System;
using System.Collections.Generic;

namespace iOverlayer.Config
{
    [Serializable]
    public class OverlayConfig
    {
        public string id;
        public string text;
        public float x;
        public float y;
        public int fontSize;
        public string color;
        public string font;
        public bool enabled;
    }

    [Serializable]
    public class OverlayConfigFile
    {
        public string version;
        public List<OverlayConfig> overlays;
    }
}

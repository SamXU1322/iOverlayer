using iOverLayer.Text;
namespace iOverLayer.ADOFAI
{
    public static class BPM
    {
        private static float _lastTileBPM = -1;
        private static float _lastCurBPM = -1;
        private static float _tbpm = -1f;
        private static float _cbpm = -1f;
        private static float _kps = -1f;
        public static float TBPM => _tbpm;
        public static float CBPM => _cbpm;
        public static float kps => _kps;


        public static void UpdateBPM()
        {
            scrFloor floor = scrController.instance.currFloor ?? scrController.instance.firstFloor;
            scrConductor conductor = scrConductor.instance;
            float _tbpm = (float)(conductor.bpm * conductor.song.pitch * scrController.instance.speed);
            float _cbpm = floor.nextfloor ? (float)(60.0 / (floor.nextfloor.entryTime - floor.entryTime) * conductor.song.pitch) : _tbpm;
            float _kps = _cbpm / 60;
            if (TextManager.Texts.ContainsKey("BPM"))
            {
                TextManager.Texts["BPM"].SetText(" BPM: " + _tbpm + " \nCBPM: " + _cbpm + " \nKPS: " + _kps);
            }
            else
            {
                LogSystem.Warning("the bpm text is not existed.please create it");
            }
            if (_lastTileBPM == _tbpm && _lastCurBPM == _cbpm) return;
            _lastTileBPM = _tbpm;
            _lastCurBPM = _cbpm;
        }
    }
}

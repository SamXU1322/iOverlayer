using ADOFAI;

namespace iOverLayer.ADOFAI
{
    internal static class Title
    {
        public static LevelData levelData = scnGame.instance?.levelData ?? scnEditor.instance?.levelData;
        public static string _titleRaw;
        public static string _authorRaw;
        public static string _artistRaw;
        public static void LevelInit()
        {
            if(scnGame.instance != null)
            {
                _titleRaw = levelData?.song;
                _authorRaw = levelData?.author;
                _artistRaw = levelData?.artist;
            }
            else
            {
                _titleRaw = ADOBase.sceneName;
                _artistRaw = string.Empty;
                _authorRaw = string.Empty;
            }
            LogSystem.Info($"Title: {_titleRaw}, Author: {_authorRaw}, Artist: {_artistRaw}");
        }
    }
}

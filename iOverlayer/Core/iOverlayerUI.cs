using UnityEngine;
using UnityModManagerNet;

namespace iOverlayer.Core
{
    public class iOverlayerUI:MonoBehaviour
    {
        private static float _Width = 300f;
        private static float _Height = 200f;
        private static bool _IsShow = false;
        private static Rect _rect;
        private static Vector2 _lastMousePosition;
        private UnityModManager.ModEntry.ModLogger _logger;
        public void Initilize(UnityModManager.ModEntry.ModLogger logger)
        {
            _logger = logger;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
                _rect = new Rect(mousePosition.x, mousePosition.y, _Width, _Height);
                _IsShow = true;
                _logger.Log($"右键菜单在位置 {mousePosition} 打开");
            }
        }
        public void OnGUI()
        {
            if(!_IsShow)return;
            Rect adjustedRect = new Rect(
                Mathf.Clamp(_rect.x,0,Screen.width-_Width),
                Mathf.Clamp(_rect.y,0,Screen.height-_Height),
                _Width,
                _Height
                );
            GUI.Window(801, adjustedRect, DrawWindow, "iOverlayer 右键菜单");
        }
        private void DrawWindow(int windowID)
        { 
            GUILayout.BeginVertical();
            GUILayout.Label("请选择功能", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 16,
                    fontStyle = FontStyle.Bold
            });
            GUILayout.Space(10);
            if(GUILayout.Button("功能一",GetButtonStyle()))
            {
                _logger.Log("功能一被点击");
            }
            GUILayout.Space(15);
            if (GUILayout.Button("关闭", GetButtonStyle()))
            {
                CloseUI();
            }
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0,0,10000,10000));
        }
        private GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                margin = new RectOffset(5,5,5,5),
                padding = new RectOffset(10,10,8,8),
                fixedHeight = 35f
            };
        }
        public void CloseUI()
        {
            _IsShow = false;
        }
    }
}
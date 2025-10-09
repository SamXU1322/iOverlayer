using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace iOverlayer
{
    public class KeyManager:MonoBehaviour
    {
        private static KeyManager _instance;
        public static KeyManager Instance => _instance;
        private UnityModManager.ModEntry.ModLogger _logger;
        private bool _isEnabled = true;
        private Dictionary<KeyCode, string> _keyBindings = new Dictionary<KeyCode, string>();
        // def __init__(self
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Initialize(UnityModManager.ModEntry modEntry)
        {
            _logger = modEntry.Logger;
            SetupDefaultKeyBindings();
        }
        private void SetupDefaultKeyBindings()
        {
            _keyBindings[KeyCode.F1] = "显示界面";
        }

        void Update()
        {
            if(!_isEnabled) return;
            DetectKeys();
        }
        private void DetectKeys()
        {
            foreach (var binding in _keyBindings)
            {
                if (Input.GetKeyDown(binding.Key))
                {
                    _logger.Log($"{binding.Key} - {binding.Value}");
                }
                if (Input.GetMouseButtonDown(0))
                    _logger.Log("鼠标左键点击");
                
                if (Input.GetMouseButtonDown(1))
                    _logger.Log("鼠标右键点击");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityModManagerNet;

namespace iOverlayer
{
    public class KeyManager:MonoBehaviour
    {
        private static KeyManager _instance;
        public static KeyManager Instance => _instance;
        private UnityModManager.ModEntry.ModLogger logger;
        private bool isEnabled = true;
        private Dictionary<KeyCode, string> keyBindings = new Dictionary<KeyCode, string>();

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

        public void Initialtize(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;
            SetupDefaultKeyBingings();
        }
        private void SetupDefaultKeyBingings()
        {
            keyBindings[KeyCode.F1] = "显示界面";
        }

        void Update()
        {
            if(!isEnabled) return;
            DetectKeys();
        }
        private void DetectKeys()
        {
            foreach (var binding in keyBindings)
            {
                if (Input.GetKeyDown(binding.Key))
                {
                    logger.Log($"{binding.Key} - {binding.Value}");
                }
                if (Input.GetMouseButtonDown(0))
                    logger.Log("鼠标左键点击");
                
                if (Input.GetMouseButtonDown(1))
                    logger.Log("鼠标右键点击");
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace LightPat.Core
{
    public class DisplayLogger : MonoBehaviour
    {
        private static DisplayLogger _instance;

        [SerializeField]
        private TextMeshProUGUI debugAreaText = null;

        private int maxLines = 15;

        public static DisplayLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("Logger is null");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;

            debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled</color>\n";
        }

        public void LogInfo(string message)
        {
            ClearLines();

            debugAreaText.text += $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        }

        private void ClearLines()
        {
            if (debugAreaText.text.Split('\n').Length >= maxLines)
            {
                debugAreaText.text = string.Empty;
            }
        }
    }
}
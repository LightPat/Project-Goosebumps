using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace LightPat.UI
{
    public class ControlsSettingsMenu : MonoBehaviour
    {
        public TMP_InputField mouseSensitivity;

        private void Start()
        {
            mouseSensitivity.text = transform.parent.GetComponent<EscapeMenu>().playerController.sensitivity.ToString();
        }

        public void OnSensitivityChanged()
        {
            transform.parent.GetComponent<EscapeMenu>().playerController.sensitivity = float.Parse(mouseSensitivity.text);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
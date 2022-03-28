using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LightPat.Core;
using System;

namespace LightPat.UI
{
    public class EscapeMenu : MonoBehaviour
    {
        public GameObject SettingsMenu;

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenSettingsMenu()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Instantiate(SettingsMenu, transform);
        }
    }
}
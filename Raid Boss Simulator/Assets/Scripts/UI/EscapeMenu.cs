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
        public TMP_Dropdown resolutionDropdown, fullscreenModeDropdown;

        private FullScreenMode[] fsModes = new FullScreenMode[3];
        private List<Resolution> supportedResolutions = new List<Resolution>();

        public void QuitGame()
        {
            Application.Quit();
        }

        public void OpenSettingsMenu()
        {
            GameObject SettingsMenuParent = transform.Find("Settings Menu").gameObject;

            // Resolution Dropdown
            List<string> resolutionOptions = new List<string>();

            int currentResIndex = 0;
            for (int i = 0; i < Screen.resolutions.Length; i++)
            {
                // If the resolution is 16:9
                if ((Screen.resolutions[i].width * 9 / Screen.resolutions[i].height) == 16 & Screen.currentResolution.refreshRate == Screen.resolutions[i].refreshRate)
                {
                    resolutionOptions.Add(Screen.resolutions[i].ToString());
                    supportedResolutions.Add(Screen.resolutions[i]);
                }

                if (Screen.resolutions[i].Equals(Screen.currentResolution))
                {
                    currentResIndex = resolutionOptions.Count-1;
                }
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = currentResIndex;

            // Full screen mode dropdown
            // Dropdown Options are assigned in inspector since these don't vary
            fsModes[0] = FullScreenMode.ExclusiveFullScreen;
            fsModes[1] = FullScreenMode.FullScreenWindow;
            fsModes[2] = FullScreenMode.Windowed;
            int fsModeIndex = Array.IndexOf(fsModes, Screen.fullScreenMode);
            fullscreenModeDropdown.value = fsModeIndex;

            SettingsMenuParent.SetActive(true);
            transform.Find("Settings Button").gameObject.SetActive(false);
            transform.Find("Quit Button").localPosition = new Vector3(0, -400, 0);
        }

        public void ApplyChanges()
        {
            // Fullscreen Dropdown
            FullScreenMode fsMode = fsModes[fullscreenModeDropdown.value];

            // Resolution Dropdown
            // Options are assigned automatically in OpenSettingsMenu()
            Resolution res = supportedResolutions[resolutionDropdown.value];

            Screen.SetResolution(res.width, res.height, fsMode, res.refreshRate);
        }
    }
}
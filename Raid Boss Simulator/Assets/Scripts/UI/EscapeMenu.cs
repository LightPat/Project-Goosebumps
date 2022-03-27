using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using LightPat.Core;

namespace LightPat.UI
{
    public class EscapeMenu : MonoBehaviour
    {
        private Resolution[] resolutions;

        private void Start()
        {
            resolutions = Screen.resolutions;
        }

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
            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionOptions.Add(resolutions[i].ToString());

                if (resolutions[i].ToString() == Screen.currentResolution.ToString())
                {
                    currentResIndex = i;
                }
            }

            SettingsMenuParent.transform.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>().ClearOptions();
            SettingsMenuParent.transform.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>().AddOptions(resolutionOptions);
            SettingsMenuParent.transform.Find("Resolution Dropdown").GetComponent<TMP_Dropdown>().value = currentResIndex;

            SettingsMenuParent.SetActive(true);
            transform.Find("Settings Button").gameObject.SetActive(false);
        }

        public void OnResolutionDropdownChanged(TMP_Dropdown dropdown)
        {
            Resolution res = resolutions[dropdown.value];

            Debug.Log(res.ToString());

            Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow, res.refreshRate);
        }
    }
}
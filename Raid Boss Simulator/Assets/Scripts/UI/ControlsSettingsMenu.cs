using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsSettingsMenu : MonoBehaviour
{
    public void Start()
    {
        Debug.Log("Opening");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

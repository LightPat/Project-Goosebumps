using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkInterface : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    void Start()
    {
        // Updates the GUI whenever a client connects
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            playersInGameText.SetText("Players Connected: " + (id+1).ToString());
            Logger.Instance.LogInfo($"{id} just connected...");
        };
    }

    public void startHost()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Logger.Instance.LogInfo("Host started...");
        }
        else
        {
            Logger.Instance.LogInfo("Unable to start host...");
        }
    }

    public void startClient()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Logger.Instance.LogInfo("Client started...");
        }
        else
        {
            Logger.Instance.LogInfo("Unable to start client...");
        }
    }

    public void startServer()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Logger.Instance.LogInfo("Server started...");
        }
        else
        {
            Logger.Instance.LogInfo("Unable to start server...");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
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

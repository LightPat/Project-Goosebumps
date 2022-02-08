using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class NetworkInterface : NetworkBehaviour
{
    public GameObject ServerCamera;

    [SerializeField]
    private TextMeshProUGUI playersInGameText;

    NetworkVariable<int> playersInGame = new NetworkVariable<int>();

    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                playersInGame.Value++;
                //Logger.Instance.LogInfo(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject.name);
            }

            Logger.Instance.LogInfo($"{id} just connected...");
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            if (IsServer)
            {
                playersInGame.Value--;
            }

            Logger.Instance.LogInfo($"{id} just disconnected...");
        };
    }

    void Update()
    {
        playersInGameText.SetText(playersInGame.Value.ToString() + " Players Connected");
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

        ServerCamera.SetActive(true);
    }
}

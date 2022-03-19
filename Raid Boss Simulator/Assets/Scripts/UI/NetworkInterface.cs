using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using LightPat.Core;

namespace LightPat.UI
{
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
                //DisplayLogger.Instance.LogInfo(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject.name);
                }

                DisplayLogger.Instance.LogInfo($"{id} just connected...");
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                if (IsServer)
                {
                    playersInGame.Value--;
                }

                DisplayLogger.Instance.LogInfo($"{id} just disconnected...");
            };
        }

        void Update()
        {
            playersInGameText.SetText(playersInGame.Value.ToString() + " Players Connected");
        }

        public void startHost()
        {
            string IPAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;

            if (IPAddress != "")
            {
                NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;
            }

            if (NetworkManager.Singleton.StartHost())
            {
                DisplayLogger.Instance.LogInfo("Host started...");

                DisplayLogger.Instance.LogInfo("Server started on " + NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress
                    + " on port " + NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ServerListenPort);

                foreach (Transform child in transform)
                {
                    if (child.transform.childCount > 0)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            else
            {
                DisplayLogger.Instance.LogInfo("Unable to start host...");
            }
        }

        public void startClient()
        {
            string IPAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;

            if (IPAddress != "")
            {
                NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;
            }

            if (NetworkManager.Singleton.StartClient())
            {
                DisplayLogger.Instance.LogInfo("Client started...");

                foreach (Transform child in transform)
                {
                    if (child.transform.childCount > 0)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            else
            {
                DisplayLogger.Instance.LogInfo("Unable to start client...");
            }
        }

        public void startServer()
        {
            string IPAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;

            if (IPAddress != "")
            {
                NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;
            }

            if (NetworkManager.Singleton.StartServer())
            {
                DisplayLogger.Instance.LogInfo("Server started on " + NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress
                    + " on port " + NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ServerListenPort);

                foreach (Transform child in transform)
                {
                    if (child.transform.childCount > 0)
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
            else
            {
                DisplayLogger.Instance.LogInfo("Unable to start server...");
            }

            ServerCamera.SetActive(true);
        }
    }
}
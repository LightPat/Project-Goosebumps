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
        public GameObject ServerObjects;

        [SerializeField]
        private TextMeshProUGUI playersInGameText;

        NetworkVariable<int> playersInGame = new NetworkVariable<int>();

        void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                string playerName = System.Text.Encoding.ASCII.GetString(NetworkManager.Singleton.NetworkConfig.ConnectionData);

                if (IsServer)
                {
                    playersInGame.Value++;

                    NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject.GetComponent<PlayerController>().playerName = playerName;

                    //DisplayLogger.Instance.LogInfo(playerName);

                    //DisplayLogger.Instance.LogInfo(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject.GetComponent<PlayerController>().playerName);
                    //DisplayLogger.Instance.LogInfo(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.gameObject.name);
                }

                DisplayLogger.Instance.LogInfo($"{playerName} just connected...");
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

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            DisplayLogger.Instance.LogInfo(connectionData.Length.ToString());

            //Your logic here
            bool approve = true;
            bool createPlayerObject = true;

            // Position to spawn the player object at, set to null to use the default position
            Vector3? positionToSpawnAt = Vector3.zero;

            // Rotation to spawn the player object at, set to null to use the default rotation
            Quaternion rotationToSpawnWith = Quaternion.identity;

            //If approve is true, the connection gets added. If it's false. The client gets disconnected
            callback(createPlayerObject, null, approve, positionToSpawnAt, rotationToSpawnWith);
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
            string playerName = transform.Find("Player Name").GetComponent<TMP_InputField>().text;

            if (IPAddress != "")
            {
                NetworkManager.gameObject.GetComponent<Unity.Netcode.Transports.UNET.UNetTransport>().ConnectAddress = transform.Find("IP Address").GetComponent<TMP_InputField>().text;
            }

            if (playerName != "")
            {
                NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(playerName);
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

            ServerObjects.SetActive(true);
        }
    }
}
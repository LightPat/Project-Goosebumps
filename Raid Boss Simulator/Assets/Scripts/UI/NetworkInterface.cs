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
        private NetworkVariable<int> playersInGame = new NetworkVariable<int>();
        private Dictionary<ulong, string> clientNames = new Dictionary<ulong, string>();

        void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                if (IsServer)
                {
                    playersInGame.Value++;

                    for (ulong i = 1; i <= id; i++)
                    {
                        NetworkManager.Singleton.ConnectedClients[i].PlayerObject.gameObject.GetComponent<PlayerController>().updateName(clientNames[i]);
                    }

                    DisplayLogger.Instance.LogInfo(clientNames[id] + " just connected...");
                }

                
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                if (IsServer)
                {
                    playersInGame.Value--;

                    DisplayLogger.Instance.LogInfo(clientNames[id] + " just disconnected...");

                    clientNames.Remove(id);
                }
            };
        }

        private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            string playerName = System.Text.Encoding.ASCII.GetString(connectionData);
            clientNames.Add(clientId, playerName);

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

                if (ServerCamera.GetComponent<ObjectsToSpawn>())
                {
                    ServerCamera.GetComponent<ObjectsToSpawn>().SpawnObjects();
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

            if (playerName == "")
            {
                NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("Player");
            }
            else
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

                ServerCamera.SetActive(true);

                if (ServerCamera.GetComponent<ObjectsToSpawn>())
                {
                    ServerCamera.GetComponent<ObjectsToSpawn>().SpawnObjects();
                }
            }
            else
            {
                DisplayLogger.Instance.LogInfo("Unable to start server...");
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
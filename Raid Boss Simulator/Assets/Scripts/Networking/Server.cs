using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    public void moveClient(ulong clientId, Vector3 newClientPosition)
    {
        //MoveClientRpc(clientId, newClientPosition);

        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            // First index is the client Id
            // Second index is the network client object
            if (client.Key == clientId)
            {
                // PlayerObject is the network object component
                GameObject player = client.Value.PlayerObject.gameObject;

                Logger.Instance.LogInfo(clientId.ToString() + " " + newClientPosition.ToString());
                player.GetComponent<Rigidbody>().MovePosition(newClientPosition);
            }
        }
    }

    [ClientRpc]
    void MoveClientRpc(ulong clientId, Vector3 newClientPosition)
    {
        // Update this client's position everywhere except the original
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                Logger.Instance.LogInfo(clientId.ToString() + " " + newClientPosition.ToString());
                player.GetComponent<Rigidbody>().MovePosition(newClientPosition);
            }
        }
    }
}

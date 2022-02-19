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

                player.transform.position = newClientPosition;
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
                player.transform.position = newClientPosition;
            }
        }
    }

    public void rotateClient(ulong clientId, Vector3 newClientRotationEulers)
    {
        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key == clientId)
            {
                GameObject player = client.Value.PlayerObject.gameObject;

                //player.transform.rotation = Quaternion.Euler(0, newClientRotationEulers.x, 0);
                player.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, newClientRotationEulers.x, 0));
                player.transform.Find("Vertical Rotate").rotation = Quaternion.Euler(-newClientRotationEulers.y, newClientRotationEulers.x, 0);
            }
        }
    }

    void Update()
    {
        if (IsServer)
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                GameObject player = client.Value.PlayerObject.gameObject;

                //Debug.Log(player.transform.rotation.eulerAngles.ToString());
            }
        }
        
    }
}

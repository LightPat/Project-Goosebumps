using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    private float currentSpeed = 5f;
    private float sensitivity = 15f;

    private Vector2 lookInput;
    
    public void recieveClientInput(ulong clientId, Vector2 input, string action)
    {
        foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        {
            // First index is the client Id
            // Second index is the network client object
            if (client.Key == clientId)
            {
                // PlayerObject is the network object component
                GameObject player = client.Value.PlayerObject.gameObject;

                // At this point, player is the object that want to perform the action on, and input is the amount
                switch (action)
                {
                    case "Move":
                        Rigidbody rb = player.GetComponent<Rigidbody>();
                        Vector3 newClientPosition = player.transform.position + rb.rotation * new Vector3(input.x, 0, input.y) * currentSpeed * NetworkManager.Singleton.LocalTime.FixedDeltaTime;
                        rb.MovePosition(newClientPosition);
                        // Propogate change to clients now too
                        break;
                    case "Look":
                        input *= (sensitivity * NetworkManager.Singleton.LocalTime.FixedDeltaTime);
                        // TODO Implement camera bounds
                        player.GetComponent<Rigidbody>().MoveRotation(player.transform.rotation * Quaternion.Euler(0, input.x, 0));
                        //player.transform.Find("Vertical Rotate").Rotate(-input.y, input.x, 0);
                        break;
                    default:
                        Debug.Log("The server doesn't know what to do with this action!\n" + action);
                        break;
                }
            }
        }

        //Debug.Log(NetworkManager.Singleton.ConnectedClientsList[clientId]);
        //Logger.Instance.LogInfo(NetworkManager.Singleton.ConnectedClientsList[clientId].PlayerObject.gameObject.ToString());

        // Add rotation transform.position + rb.rotation

        //rb.MovePosition(newPosition);
    }

    [ClientRpc]
    void UpdatePlayerPositionClientRpc()
    {
        //Logger.Instance.LogInfo(networkPosition.ToString());



        //// This code is executed on each client
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        //foreach (GameObject player in players)
        //{
        //    // Update the player object if they aren't the local player, since the local player moves itself
        //    if (!player.GetComponent<NetworkObject>().IsLocalPlayer)
        //    {
        //        Vector3 newClientPosition = player.GetComponent<PlayerController>().networkPosition.Value;

        //        player.transform.position = newClientPosition;
        //    }
        //}
    }

    //[ClientRpc]
    //void UpdatePlayerRotationClientRpc()
    //{
    //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

    //    foreach (GameObject player in players)
    //    {
    //        if (!player.GetComponent<NetworkObject>().IsLocalPlayer)
    //        {
    //            Quaternion newClientRotation = player.GetComponent<PlayerController>().networkRotation.Value.normalized;

    //            Logger.Instance.LogInfo(player.GetComponent<NetworkObject>().OwnerClientId + " " + newClientRotation.ToString());

    //            //player.GetComponent<Rigidbody>().MoveRotation(newClientRotation);
    //            player.transform.rotation = newClientRotation;
    //        }
    //    }
    //}
}

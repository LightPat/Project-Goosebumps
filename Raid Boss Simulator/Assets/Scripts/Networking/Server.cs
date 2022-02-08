using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Server : NetworkBehaviour
{
    [Header("Player Controller")]
    private float currentSpeed = 5f;
    private float sensitivity = 15f;
    public float jumpHeight = 3f;
    public float fallingGravityScale = 0.5f;

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
                        ActionClientRpc(clientId, newClientPosition);
                        break;
                    case "Look":
                        input *= (sensitivity * NetworkManager.Singleton.LocalTime.FixedDeltaTime);
                        // TODO Implement camera bounds
                        Quaternion newClientRotation = player.transform.rotation * Quaternion.Euler(0, input.x, 0);
                        player.GetComponent<Rigidbody>().MoveRotation(newClientRotation);
                        ActionClientRpc(clientId, newClientRotation);
                        break;
                    case "Jump":
                        float jumpForce = Mathf.Sqrt(jumpHeight * -2 * Physics.gravity.y);
                        player.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                        JumpClientRpc(clientId, jumpForce);
                        break;
                    default:
                        Debug.Log("The server doesn't know what to do with this action!\n" + action);
                        Logger.Instance.LogInfo("The server doesn't know what to do with this action!\n" + action);
                        break;
                }
                return;
            }
        }
    }

    [ClientRpc]
    void ActionClientRpc(ulong clientId, Vector3 newClientPosition)
    {
        // Find player object of client and perform an action on that object
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                player.GetComponent<Rigidbody>().MovePosition(newClientPosition);
            }
        }
    }

    [ClientRpc]
    void ActionClientRpc(ulong clientId, Quaternion newClientRotation)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                player.GetComponent<Rigidbody>().MoveRotation(newClientRotation);
            }
        }
    }

    [ClientRpc]
    void JumpClientRpc(ulong clientId, float jumpForce)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                player.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
            }
        }
    }

    //[ClientRpc]
    //void TestClientRpc()
    //{
    //    return;
        //foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
        //{
        //    if (client.Key == clientId)
        //    {
        //        float jumpForce = Mathf.Sqrt(3 * -2 * Physics.gravity.y);

        //        Debug.Log(client.Value.PlayerObject);
        //        //client.Value.PlayerObject.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        //        Logger.Instance.LogInfo("Jumping");
        //    }
        //}
    //}

    //[ClientRpc]
    //void UpdatePlayerPositionClientRpc()
    //{
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
    //}

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

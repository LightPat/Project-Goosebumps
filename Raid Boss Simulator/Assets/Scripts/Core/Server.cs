using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using LightPat.Core.WeaponSystem;

namespace LightPat.Core
{
    /// <summary>
    /// Every public method has a corresponding ClientRpc private method, which propogates the change that occured on the server side to each client
    /// </summary>
    public class Server : NetworkBehaviour
    {
        private static Server _instance;

        public static Server Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.Log("Server is null");
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        public void moveClient(ulong clientId, Vector3 newClientPosition)
        {
            MoveClientRpc(clientId, newClientPosition);

            if (IsHost) { return; }

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                // First index is the client Id
                // Second index is the network client object
                if (client.Key == clientId)
                {
                    // PlayerObject is the network object component
                    GameObject player = client.Value.PlayerObject.gameObject;

                    player.transform.position = newClientPosition;
                    break;
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
                // If this gameObject ISN'T the client who moved, and the client Ids match, propagate the move change
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId & !player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    player.transform.position = newClientPosition;
                    break;
                }
            }
        }

        public void rotateClient(ulong clientId, Vector3 newClientRotationEulers)
        {
            RotateClientRpc(clientId, newClientRotationEulers);

            if (IsHost) { return; }

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key == clientId)
                {
                    GameObject player = client.Value.PlayerObject.gameObject;

                    player.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, newClientRotationEulers.x, 0));
                    player.GetComponent<PlayerController>().verticalRotate.rotation = Quaternion.Euler(-newClientRotationEulers.y, newClientRotationEulers.x, 0);
                    break;
                }
            }
        }

        [ClientRpc]
        void RotateClientRpc(ulong clientId, Vector3 newClientRotationEulers)
        {
            // Update this client's position everywhere except the original
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId & !player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    player.GetComponent<Rigidbody>().MoveRotation(Quaternion.Euler(0, newClientRotationEulers.x, 0));
                    player.GetComponent<PlayerController>().verticalRotate.rotation = Quaternion.Euler(-newClientRotationEulers.y, newClientRotationEulers.x, 0);
                    break;
                }
            }
        }

        public void clientJump(ulong clientId, float jumpForce)
        {
            JumpClientRpc(clientId, jumpForce);

            if (IsHost) { return; }

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key == clientId)
                {
                    GameObject player = client.Value.PlayerObject.gameObject;

                    player.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                    break;
                }
            }
        }

        [ClientRpc]
        void JumpClientRpc(ulong clientId, float jumpForce)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId & !player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    player.GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
                    break;
                }
            }
        }

        public void clientAttack(ulong clientId)
        {
            AttackClientRpc(clientId);

            if (IsHost) { return; }

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key == clientId)
                {
                    GameObject player = client.Value.PlayerObject.gameObject;

                    player.GetComponent<WeaponLoadout>().getEquippedWeapon().GetComponent<Weapon>().attack();
                    break;
                }
            }
        }

        [ClientRpc]
        void AttackClientRpc(ulong clientId)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId & !player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    player.GetComponent<WeaponLoadout>().getEquippedWeapon().GetComponent<Weapon>().attack();
                    break;
                }
            }
        }
    }
}
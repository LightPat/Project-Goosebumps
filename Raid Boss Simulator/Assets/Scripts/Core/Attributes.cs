using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace LightPat.Core
{
    public class Attributes : NetworkBehaviour
    {
        public TextMeshProUGUI displayHP;
        public int maxHealth = 100;

        private NetworkVariable<int> HP = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            HP.OnValueChanged += HPChanged;

            if (IsServer)
            {
                HP.Value = maxHealth;
            }
        }

        void HPChanged(int oldHP, int newHP)
        {
            displayHP.SetText(HP.Value.ToString() + " HP");

            if (HP.Value <= 0)
            {
                //GetComponent<NetworkObject>().Despawn();
                DisplayLogger.Instance.LogInfo(gameObject.name + " is dead.");
                HP.Value += maxHealth;
                transform.position = new Vector3(0, 1, 0);
            }
        }

        public void changeHP(int changeValue)
        {
            changeHPServerRpc(changeValue);
        }

        [ServerRpc(RequireOwnership = false)]
        void changeHPServerRpc(int changeValue)
        {
            // Only change HP on server side, then netcode for gameObjects propogates the change throughout the network
            HP.Value += changeValue;
        }
    }
}
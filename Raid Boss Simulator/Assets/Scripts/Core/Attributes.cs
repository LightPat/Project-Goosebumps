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

        NetworkVariable<int> HP = new NetworkVariable<int>();

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
                //gameObject.SetActive(false);
                DisplayLogger.Instance.LogInfo(gameObject.name + " is dead.");
            }
        }

        public void changeHealth(int damage)
        {
            HP.Value += damage;
        }

        public int getHealth()
        {
            return HP.Value;
        }
    }
}
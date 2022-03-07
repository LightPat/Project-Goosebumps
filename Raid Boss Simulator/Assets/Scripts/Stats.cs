using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Stats : NetworkBehaviour
{
    public TextMeshProUGUI displayHP;
    public int maxHealth = 100;

    NetworkVariable<int> HP = new NetworkVariable<int>();

    void Start()
    {
        if (IsServer)
        {
            HP.Value = maxHealth;
        }

        displayHP.SetText(HP.Value.ToString() + " HP");
    }

    /// <summary>
    /// Changes the HP of this object, if the object's HP drops to 0 or below, we disable it in the scene
    /// </summary>
    /// <param name="damage"></param>
    public void changeHealth(int damage)
    {
        if (!IsServer) { return; }

        HP.Value += damage;

        if (HP.Value <= 0)
        {
            //gameObject.SetActive(false);
            Logger.Instance.LogInfo(gameObject.name + " is dead.");
        }

        displayHP.SetText(HP.Value.ToString() + " HP");
    }

    public int getHealth()
    {
        return HP.Value;
    }
}

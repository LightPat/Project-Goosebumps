using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI displayHP;
    public int maxHealth = 100;

    private int HP;

    void Start()
    {
        HP = maxHealth;
        displayHP.SetText(HP.ToString() + " HP");
    }

    /// <summary>
    /// Changes the HP of this object, if the object's HP drops to 0 or below, we disable it in the scene
    /// </summary>
    /// <param name="damage"></param>
    public void changeHealth(int damage)
    {
        HP += damage;

        if (HP <= 0)
        {
            //gameObject.SetActive(false);
            Logger.Instance.LogInfo(gameObject.name + " is dead.");
        }

        displayHP.SetText(HP.ToString() + " HP");
    }

    public int getHealth()
    {
        return HP;
    }
}

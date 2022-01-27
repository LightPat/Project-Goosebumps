using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int maxHealth = 100;

    private int HP;

    void Start()
    {
        HP = maxHealth;
    }

    public int getHealth()
    {
        return HP;
    }
}

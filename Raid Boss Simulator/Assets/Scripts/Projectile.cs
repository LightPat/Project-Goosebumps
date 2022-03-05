using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public int damage = 0;

    void OnCollisionEnter(Collision collision)
    {
        // If a projectile is hitting me, deduct the damage from my HP
        if (collision.gameObject.GetComponent<Stats>())
        {
            Logger.Instance.LogInfo("Getting hit for " + damage.ToString());
            collision.gameObject.GetComponent<Stats>().changeHealth(damage);
        }
    }
}

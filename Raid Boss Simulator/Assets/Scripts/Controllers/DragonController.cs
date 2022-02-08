using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : Controller
{
    new void Start()
    {
        base.Start();
    }

    void OnCollisionEnter(Collision collision)
    {
        // If a projectile is hitting me, deduct the damage from my HP
        if (collision.gameObject.GetComponent<Projectile>())
        {
            GetComponent<Stats>().changeHealth(collision.gameObject.GetComponent<Projectile>().damage);
        }
    }
}

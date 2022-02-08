using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public class Wand : Weapon
    {
        [Header("Sword Configuration")]
        public float attackCooldown;

        public override void attack()
        {
            if (allowAttack)
            {
                // Raycast hit detection from crosshair to enemy
                RaycastHit hit;
                bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit);

                if (bHit)
                {
                    Debug.Log(hit.collider);
                }

                // Fire rate handling
                StartCoroutine(SwingCooldownCoroutine());
            }
        }

        IEnumerator SwingCooldownCoroutine()
        {
            allowAttack = false;

            // Play animation

            yield return new WaitForSeconds(attackCooldown);
            allowAttack = true;
        }
    }
}

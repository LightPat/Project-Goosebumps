using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ItemSystem
{
    public class Gun : Weapon
    {
        [Header("Gun Configuration")]
        public float thrust;
        // This is in RPM (rounds per minute)
        public float rateOfFire;

        private LineRenderer lineRenderer;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public override void attack()
        {
            // Raycast hit detection from crosshair to enemy
            RaycastHit hit;
            bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit);

            // Apply force to object if it has a rigidbody, if it is a player, destroy it
            if (bHit)
            {
                // If we hit a player, notify that player and deduct HP from it
                if (hit.collider.gameObject.tag == "Player")
                {
                    if (hit.collider.name == "Head")
                    {
                        hit.collider.gameObject.GetComponent<Stats>().takeDamage((int)(baseDamage * headshotMultiplier));
                    }
                    else
                    {
                        hit.collider.gameObject.GetComponent<Stats>().takeDamage((int)baseDamage);
                    }
                }
            }

            // Fire rate handling
            StartCoroutine(FireRateCoroutine());
        }

        IEnumerator FireRateCoroutine()
        {
            allowAttack = false;
            // Rounds per second
            float sps = rateOfFire / 60;
            // Seconds between swings
            float seconds = 1 / sps;

            // Bullet tracer
            StartCoroutine(BulletTracer());

            yield return new WaitForSeconds(seconds);
            allowAttack = true;
            lineRenderer.enabled = false;
        }

        IEnumerator BulletTracer()
        {
            Debug.Log(gameObject.GetComponent<Collider>());

            lineRenderer.enabled = true;
            lineRenderer.material.color = Color.red;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, firstPersonCamera.transform.position + (firstPersonCamera.transform.forward * 100));

            yield return new WaitForSeconds(1);
        }
    }
}

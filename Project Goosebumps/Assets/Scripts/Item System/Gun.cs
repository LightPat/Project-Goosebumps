using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class Gun : Weapon
    {
        [Header("Gun Configuration")]
        public float rateOfFire;

        private LineRenderer lineRenderer;
        private AudioSource gunshotSound;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            gunshotSound = GetComponent<AudioSource>();
        }

        public override void attack()
        {
            if (allowAttack)
            {
                // Raycast hit detection from crosshair to enemy
                RaycastHit hit;
                bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit);

                Vector3 tracerPosition = firstPersonCamera.transform.position + (firstPersonCamera.transform.forward * 100);
                Color tracerColor = Color.red;

                if (bHit)
                {
                    // Spawn bullet which triggers on collider enter
                    // Damage handling is done on the target via OnTriggerEnter()
                    GameObject firedBullet = GameObject.Instantiate(bullet, hit.collider.transform.position, Quaternion.identity);

                    // Calculate damage here
                    firedBullet.GetComponent<Projectile>().damage = (int)baseDamage * -1;

                    // Delay destruction by a few frames so that the collision is detected by Unity
                    StartCoroutine(DelayedDestroy(firedBullet, 3));

                    tracerPosition = hit.point;
                    tracerColor = Color.green;
                }

                // Fire rate handling
                StartCoroutine(FireRateCoroutine(tracerPosition, tracerColor));
            }            
        }

        IEnumerator FireRateCoroutine(Vector3 tracerPosition, Color tracerColor)
        {
            allowAttack = false;
            // Rounds per second
            float sps = rateOfFire / 60;
            // Seconds between swings
            float seconds = 1 / sps;

            //gunshotSound.Play();

            // Update Bullet line
            lineRenderer.enabled = true;
            lineRenderer.material.color = tracerColor;
            lineRenderer.SetPosition(0, transform.Find("LineSpawnPoint").position);
            lineRenderer.SetPosition(1, tracerPosition);

            yield return new WaitForSeconds(seconds);
            allowAttack = true;
            lineRenderer.enabled = false;
        }

        IEnumerator DelayedDestroy(GameObject g, int frameDelay = 2)
        {
            for (int i = 0; i < frameDelay; i++)
            {
                yield return i;
            }

            Destroy(g);
        }
    }
}

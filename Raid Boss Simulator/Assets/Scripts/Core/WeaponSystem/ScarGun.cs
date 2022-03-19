using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LightPat.Core.WeaponSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScarGun : Weapon
    {
        [Header("Gun Configuration")]
        public float rateOfFire;

        private LineRenderer lineRenderer;
        private AudioSource gunshotSound;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            gunshotSound = GetComponent<AudioSource>();
        }

        public override void attack()
        {
            if (!allowAttack) { return; }

            // Raycast hit detection from crosshair to enemy
            RaycastHit hit;
            bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit);

            Vector3 tracerPosition = firstPersonCamera.transform.position + firstPersonCamera.transform.forward * 100;
            Color tracerColor = Color.red;

            if (bHit)
            {
                if (hit.transform.gameObject.GetComponent<Attributes>())
                {
                    hit.transform.gameObject.GetComponent<Attributes>().changeHP((int)-baseDamage);
                }

                tracerPosition = hit.point;
                tracerColor = Color.green;
            }

            // Fire rate handling
            StartCoroutine(FireRateCoroutine(tracerPosition, tracerColor));
        }

        IEnumerator FireRateCoroutine(Vector3 tracerPosition, Color tracerColor)
        {
            allowAttack = false;
            // Rounds per second
            float sps = rateOfFire / 60;
            // Seconds between shots
            float seconds = 1 / sps;

            gunshotSound.Play();

            // Update Bullet line
            lineRenderer.enabled = true;
            lineRenderer.material.color = tracerColor;
            lineRenderer.SetPosition(0, transform.Find("LineSpawnPoint").position);
            lineRenderer.SetPosition(1, tracerPosition);

            yield return new WaitForSeconds(seconds);
            allowAttack = true;
            lineRenderer.enabled = false;
        }
    }
}

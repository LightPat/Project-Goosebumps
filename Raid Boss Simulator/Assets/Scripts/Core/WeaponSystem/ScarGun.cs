using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace LightPat.Core.WeaponSystem
{
    [RequireComponent(typeof(LineRenderer))]
    public class ScarGun : Weapon
    {
        [Header("Gun Configuration")]
        public float rateOfFire;
        public int magazineSize;
        [HideInInspector]
        public int bulletsRemaining;

        private LineRenderer lineRenderer;
        private AudioSource gunshotSound;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            gunshotSound = GetComponent<AudioSource>();
            gunshotSound.volume = volume;
            bulletsRemaining = magazineSize;
        }

        private void Update()
        {
            if (bulletsRemaining == 0)
            {
                reload();
            }
        }

        public override void attack()
        {
            if (!allowAttack | reloading) { return; }

            bulletsRemaining--;
            displayAmmoCount(bulletsRemaining.ToString() + " / " + magazineSize.ToString());
            allowAttack = false;
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

        public override void reload()
        {
            if (reloading | bulletsRemaining == magazineSize) { return; }

            reloading = true;

            // Check if mag is full here

            Transform magazine = transform.Find("Magazine");

            GameObject newMag = Instantiate(magazine.gameObject, transform);
            newMag.name = magazine.name;
            newMag.SetActive(false);

            magazine.parent = null;
            magazine.GetComponent<Rigidbody>().isKinematic = false;
            magazine.GetComponent<Collider>().enabled = true;
            GetComponent<Animator>().Play("Reload");

            StartCoroutine(ReloadCoroutine(magazine.gameObject, newMag));
        }

        IEnumerator ReloadCoroutine(GameObject oldMag, GameObject newMag)
        {
            yield return new WaitForSeconds(1);
            newMag.SetActive(true);
            reloading = false;
            bulletsRemaining = magazineSize;
            displayAmmoCount(bulletsRemaining.ToString() + " / " + magazineSize.ToString());

            // Destroy the old magazine that's on the ground now
            yield return new WaitForSeconds(3);
            Destroy(oldMag);
        }

        void displayAmmoCount(string ammo)
        {
            inventoryTextSlot.GetComponent<TextMeshProUGUI>().SetText(name + " " + ammo);
        }
    }
}
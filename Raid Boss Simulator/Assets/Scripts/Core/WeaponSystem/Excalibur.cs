using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LightPat.Core.WeaponSystem
{
    public class Excalibur : Weapon
    {
        [Header("Sword Configuration")]
        public float swingRate;
        public float reach;

        private AudioSource swordSlashSound;

        private void Start()
        {
            swordSlashSound = GetComponent<AudioSource>();
        }

        public override void attack()
        {
            if (!allowAttack) { return; }

            // Raycast hit detection from crosshair to enemy
            RaycastHit hit;
            bool bHit = Physics.Raycast(firstPersonCamera.transform.position, firstPersonCamera.transform.forward, out hit);

            if (bHit)
            {
                if (hit.transform.gameObject.GetComponent<Attributes>())
                {
                    changeHPServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId, (int)baseDamage * -1);
                }
            }

            StartCoroutine(SwingRateCoroutine());
        }

        IEnumerator SwingRateCoroutine()
        {
            allowAttack = false;
            // Rounds per second
            float sps = swingRate / 60;
            // Seconds between swings
            float seconds = 1 / sps;

            Debug.Log(seconds);

            swordSlashSound.Play();

            yield return new WaitForSeconds(seconds);
            allowAttack = true;
        }
    }
}
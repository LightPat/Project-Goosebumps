using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightPat.Core.WeaponSystem
{
    public class Excalibur : Weapon
    {
        [Header("Sword Configuration")]
        public float swingRate;
        public float reach;

        public override void attack()
        {
            if (!allowAttack) { return; }


        }

        IEnumerator SwingRateCoroutine(Vector3 tracerPosition, Color tracerColor)
        {
            allowAttack = false;
            // Rounds per second
            float sps = swingRate / 60;
            // Seconds between swings
            float seconds = 1 / sps;

            yield return new WaitForSeconds(seconds);
            allowAttack = true;
        }
    }
}
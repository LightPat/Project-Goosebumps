using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LightPat.Core.WeaponSystem
{
    public class WeaponFollow : MonoBehaviour
    {
        [HideInInspector]
        public Transform followTarget;

        private void Update()
        {
            if (followTarget == null) { return; }

            transform.position = Vector3.MoveTowards(transform.position, followTarget.position, 0.5f);
            //reg.transform.localPosition = reg.transform.Find("Weapon Handle").localPosition * -1;
        }
    }
}
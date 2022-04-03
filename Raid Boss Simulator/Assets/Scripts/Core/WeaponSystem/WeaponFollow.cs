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
            transform.position = followTarget.position;
            transform.localPosition = transform.localPosition + transform.Find("PrimaryHandSpot").localPosition * -1 * transform.localScale.x;
        }
    }
}
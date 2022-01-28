using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem
{
    public abstract class Weapon : MonoBehaviour
    {
        [Header("General Weapon Properties")]
        public float baseDamage;
        public float headshotMultiplier = 2f;
        public bool fullAuto = true;

        [HideInInspector]
        public bool allowAttack;
        [HideInInspector]
        public bool equipped;

        protected GameObject firstPersonCamera;

        public abstract void Attack();

        public void updateCamera()
        {
            firstPersonCamera = transform.parent.parent.Find("First Person Camera").gameObject;
        }
    }
}
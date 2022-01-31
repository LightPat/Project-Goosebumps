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
        public GameObject bullet;

        [HideInInspector]
        protected bool allowAttack = true;

        protected GameObject firstPersonCamera;

        public abstract void attack();

        public void updateCamera()
        {
            firstPersonCamera = transform.parent.parent.Find("First Person Camera").gameObject;
        }
    }
}
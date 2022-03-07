using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

namespace LightPat.Core
{
    public abstract class Weapon : NetworkBehaviour
    {
        [Header("General Weapon Properties")]
        public float baseDamage;
        public float criticalMultiplier = 2f;
        public bool fullAuto = true;

        protected GameObject inventoryTextSlot;
        protected bool allowAttack = true;
        protected GameObject firstPersonCamera;

        public abstract void attack();

        public void updateCamera()
        {
            firstPersonCamera = transform.parent.parent.Find("First Person Camera").gameObject;
        }

        public void setTextDisplay(GameObject g)
        {
            inventoryTextSlot = g;
        }

        void OnEnable()
        {
            // Set text element to bold
            if (inventoryTextSlot != null)
            {
                inventoryTextSlot.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            }
        }

        void OnDisable()
        {
            // Set text element to normal
            if (inventoryTextSlot != null)
            {
                inventoryTextSlot.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            }
        }

        void OnTransformParentChanged()
        {
            if (transform.parent != null) { return; }

            GetComponent<Rigidbody>().isKinematic = false;
            gameObject.SetActive(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace ItemSystem
{
    public class Inventory : MonoBehaviour
    {
        public int maxCapacity;

        private GameObject[] loadout = new GameObject[3];

        void OnPrimaryWeapon()
        {
            // If there is no weapon in the primary loadout slot, return
            if (loadout[0] == null) { return; }

            // If the gun is equipped and active, disable it, otherwise enable it
            if (loadout[0].activeInHierarchy)
            {
                loadout[0].SetActive(false);
            }
            else
            {
                loadout[0].SetActive(true);
            }
        }

        public void addItem(GameObject g)
        {
            ResetTransform(g);
            g.transform.SetParent(transform.Find("Vertical Rotate").Find("Equipped Weapon Spawn Point"), false);
            g.GetComponent<Rigidbody>().isKinematic = true;
            g.SetActive(false);
            loadout[0] = g;
        }

        private void ResetTransform(GameObject g)
        {
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }
    }
}

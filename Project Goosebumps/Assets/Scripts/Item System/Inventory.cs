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
            if (loadout[0].activeInHierarchy) { loadout[0].SetActive(false); return; }

            loadout[0].SetActive(true);
        }

        public void addItem(GameObject g)
        {
            ResetTransform(g);
            g.transform.SetParent(transform.Find("Equipped Weapon Spawn Point"), false);
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

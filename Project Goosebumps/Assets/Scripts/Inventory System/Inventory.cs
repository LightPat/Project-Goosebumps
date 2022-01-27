using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem
{
    public class Inventory : MonoBehaviour
    {
        public int maxCapacity;

        private GameObject[] loadout = new GameObject[3];

        public void addItem(GameObject g)
        {
            ResetTransform(g);
            g.transform.SetParent(transform, false);
            g.SetActive(false);
        }

        private static void ResetTransform(GameObject g)
        {
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
            g.transform.localScale = Vector3.one;
        }
    }
}

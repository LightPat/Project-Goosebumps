using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

namespace ItemSystem
{
    public class Inventory : MonoBehaviour
    {
        public GameObject HUDCanvas;
        public int maxCapacity;

        private GameObject[] loadout = new GameObject[3];

        void OnSlot1()
        {
            QueryLoadout(0);
        }

        void OnSlot2()
        {
            QueryLoadout(1);
        }

        void OnSlot3()
        {
            QueryLoadout(2);
        }

        public void addItem(GameObject g)
        {
            ResetTransform(g);
            g.transform.SetParent(transform.Find("Vertical Rotate").Find("Equipped Weapon Spawn Point"), false);
            g.GetComponent<Rigidbody>().isKinematic = true;
            g.SetActive(false);

            if (g.GetComponent<Weapon>())
            {
                g.GetComponent<Weapon>().updateCamera();
            }

            // Append gameobject to end of loadout
            // TODO move this functionality to a GUI
            for (int i = 0; i < loadout.Length; i++)
            {
                if (loadout[i] == null)
                {
                    loadout[i] = g;
                    HUDCanvas.transform.Find("Slot " + (i + 1).ToString()).gameObject.GetComponent<TextMeshProUGUI>().SetText(g.name);
                    break;
                }
            }
        }

        public GameObject getEquippedWeapon()
        {
            // Loops through the loadout, if any weapons are active in the scene, return that, otherwise return null
            foreach (GameObject g in loadout)
            {
                if (g != null)
                {
                    if (g.activeInHierarchy)
                    {
                        return g;
                    }
                }
            }

            return null;
        }

        private int getEquippedWeaponIndex()
        {
            for (int i = 0; i < loadout.Length; i++)
            {
                if (loadout[i] != null)
                {
                    if (loadout[i].activeInHierarchy) { return i; }
                }
            }

            return -1;
        }

        private void ResetTransform(GameObject g)
        {
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }

        private void QueryLoadout(int index)
        {
            int i = getEquippedWeaponIndex();

            // If there is no weapon in the loadout slot, return
            if (loadout[index] == null) { return; }

            // If there is a weapon active, disable it
            foreach (GameObject g in loadout)
            {
                if (g != null)
                {
                    if (g.activeInHierarchy)
                    {
                        HUDCanvas.transform.Find("Slot " + (i + 1).ToString()).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
                        g.SetActive(false);
                        // If this weapon is the same slot as we asked for, end so that we don't set active true again
                        if (g == loadout[index]) { return; }
                    }
                }
            }

            // At this point, there is no active equipped item, so we can set the queried weapon to active
            HUDCanvas.transform.Find("Slot " + (index + 1).ToString()).gameObject.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            loadout[index].SetActive(true);
        }
    }
}

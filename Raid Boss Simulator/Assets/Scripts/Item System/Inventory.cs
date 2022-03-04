using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Netcode;
using Unity.Netcode.Components;

namespace ItemSystem
{
    public class Inventory : NetworkBehaviour
    {
        public GameObject HUDCanvas;
        public GameObject GUICanvas;
        public int maxCapacity;

        private GameObject[] loadout = new GameObject[3];
        private GameObject[] HUDloadoutDisplaySlots = new GameObject[3];
        private GameObject[] GUIloadoutDisplaySlots = new GameObject[3];

        void Start()
        {
            // Get all the textmeshpro gameObjects that are used to display the loadout
            for (int i = 0; i < HUDloadoutDisplaySlots.Length; i++)
            {
                HUDloadoutDisplaySlots[i] = HUDCanvas.transform.Find("Slot " + (i+1).ToString()).gameObject;
                GUIloadoutDisplaySlots[i] = GUICanvas.transform.Find("Slot " + (i + 1).ToString()).gameObject;
            }
        }

        public void addItem(GameObject g)
        {
            // Only the server can change ownership
            Logger.Instance.LogInfo("Target ID: " + g.GetComponent<NetworkObject>().NetworkObjectId.ToString());
            addWeaponServerRpc(g.GetComponent<NetworkObject>().NetworkObjectId, GetComponent<NetworkObject>().OwnerClientId);

            //g.GetComponent<NetworkRigidbody>().enabled = false;
            //g.GetComponent<NetworkTransform>().enabled = false;
            //ResetTransform(g);

            //g.SetActive(false);

            //if (g.GetComponent<Weapon>())
            //{
            //    g.GetComponent<Weapon>().updateCamera();
            //}

            //// Append gameobject to end of loadout if loadout slot is empty
            //// TODO OTHERWISE ADD IT TO THE PLAYER'S INVENTORY IF THEY HAVE SPACE
            //for (int i = 0; i < loadout.Length; i++)
            //{
            //    if (loadout[i] == null)
            //    {
            //        loadout[i] = g;
            //        HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(g.name);
            //        loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
            //        break;
            //    }
            //}
        }

        [ServerRpc]
        void addWeaponServerRpc(ulong targetId, ulong clientId)
        {
            GameObject[] weapons = GameObject.FindGameObjectsWithTag("InventoryItem");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    Logger.Instance.LogInfo(targetId.ToString() + " " + g.name);

                    g.transform.SetParent(GetComponent<PlayerController>().verticalRotate.Find("Equipped Weapon Spawn Point(Clone)"), false);
                    ResetTransform(g);
                    g.GetComponent<Rigidbody>().isKinematic = true;

                    if (g.GetComponent<Weapon>())
                    {
                        g.GetComponent<Weapon>().updateCamera();
                    }

                    //// Append gameobject to end of loadout if loadout slot is empty
                    //// TODO OTHERWISE ADD IT TO THE PLAYER'S INVENTORY IF THEY HAVE SPACE
                    //for (int i = 0; i < loadout.Length; i++)
                    //{
                    //    if (loadout[i] == null)
                    //    {
                    //        loadout[i] = g;
                    //        HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(g.name);
                    //        loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
                    //        break;
                    //    }
                    //}

                    //g.SetActive(false);

                    
                    //g.GetComponent<NetworkObject>().ChangeOwnership(GetComponent<NetworkObject>().OwnerClientId);
                    addWeaponClientRpc(targetId, clientId);
                }
            }
        }

        [ClientRpc]
        void addWeaponClientRpc(ulong targetId, ulong clientId)
        {
            GameObject[] weapons = GameObject.FindGameObjectsWithTag("InventoryItem");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    Logger.Instance.LogInfo("Found it " + g.ToString());
                    g.GetComponent<Rigidbody>().isKinematic = true;
                    g.transform.position = transform.Find("Vertical Rotate(Clone)").Find("Equipped Weapon Spawn Point(Clone)").position;
                    g.transform.localRotation = transform.rotation;


                    // Append gameobject to end of loadout if loadout slot is empty
                    // TODO OTHERWISE ADD IT TO THE PLAYER'S INVENTORY IF THEY HAVE SPACE
                    for (int i = 0; i < loadout.Length; i++)
                    {
                        if (loadout[i] == null)
                        {
                            loadout[i] = g;
                            HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(g.name);
                            loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
                            break;
                        }
                    }

                    //g.SetActive(false);

                }
            }
        }

        // Equipping weapons logic
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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
                        g.SetActive(false);
                        // If this weapon is the same slot as we asked for, end so that we don't set active true again
                        if (g == loadout[index]) { return; }
                    }
                }
            }

            // At this point, there is no active equipped item, so we can set the queried weapon to active
            loadout[index].SetActive(true);
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

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Inventory Toggle Logic

        /// <summary>
        /// Switches between the HUD and GUI when the InventoryToggle action is triggered
        /// </summary>
        void OnInventoryToggle()
        {
            PlayerInput playerInput = GetComponent<PlayerInput>();

            if (playerInput.currentActionMap.name == "First Person")
            {
                SyncUIElementDetails(HUDCanvas, GUICanvas);

                // Enable GUI canvas
                HUDCanvas.SetActive(false);
                GUICanvas.SetActive(true);
                
                Cursor.lockState = CursorLockMode.None;
                playerInput.SwitchCurrentActionMap("Inventory");
            }
            else if (playerInput.currentActionMap.name == "Inventory")
            {
                SyncUIElementDetails(GUICanvas, HUDCanvas);

                // Enable HUD canvas
                GUICanvas.SetActive(false);
                HUDCanvas.SetActive(true);

                Cursor.lockState = CursorLockMode.Locked;
                playerInput.SwitchCurrentActionMap("First Person");
            }

            // Reset the loadout editing variables
            if (selectedSlot != null)
            {
                selectedSlot.GetComponent<Image>().color = originalColor;
                selectedSlot = null;
            }
            
            targetSlot = null;
        }

        /// <summary>
        /// Syncs the text of any objects that share the same name when switching from HUD to inventory GUI mode
        /// </summary>
        /// <param name="providerCanvas"></param>
        /// <param name="dependantCanvas"></param>
        public void SyncUIElementDetails(GameObject providerCanvas, GameObject dependantCanvas)
        {
            foreach (Transform provider in providerCanvas.transform)
            {
                foreach (Transform dependant in dependantCanvas.transform)
                {
                    if (provider.name == dependant.name)
                    {
                        // Because the GUI has buttons and the HUD has text objects
                        Transform d = dependant;
                        Transform p = provider;
                        if (dependant.GetComponent<Button>())
                        {
                            d = dependant.Find("Text (TMP)");
                        }
                        else if (provider.GetComponent<Button>())
                        {
                            p = provider.Find("Text (TMP)");
                        }

                        d.GetComponent<TextMeshProUGUI>().SetText(p.GetComponent<TextMeshProUGUI>().text);
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Editing loadout logic

        private GameObject selectedSlot = null;
        private GameObject targetSlot = null;
        Color originalColor;
        public void switchLoadoutSlotOnClick()
        {
            // If we haven't selected the slot we want to move, set that to our clicked object
            if (selectedSlot == null)
            {
                foreach (GameObject g in loadout)
                {
                    if (g != null)
                    {
                        g.SetActive(false);
                    }
                }

                selectedSlot = EventSystem.current.currentSelectedGameObject;
                originalColor = selectedSlot.GetComponent<Image>().color;
                selectedSlot.GetComponent<Image>().color = new Color32(0,0,0,100);
            }
            else
            {
                // If we already have a selected slot, set the target to our next click, and then switch the slots
                targetSlot = EventSystem.current.currentSelectedGameObject;

                int start = int.Parse(selectedSlot.name.Substring(5, 1)) - 1;
                int end = int.Parse(targetSlot.name.Substring(5, 1)) - 1;

                // Switch the location of the object in the loadout array
                GameObject temp = loadout[end];
                loadout[end] = loadout[start];
                loadout[start] = temp;

                // Update the display slot of the weapon so that bolding is applied properly
                if (loadout[start] != null)
                {
                    loadout[start].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[start]);
                }

                if (loadout[end] != null)
                {
                    loadout[end].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[end]);
                }

                syncLoadoutWithUI();

                // Reset gameObjects
                selectedSlot.GetComponent<Image>().color = originalColor;
                selectedSlot = null;
                targetSlot = null;
            }
        }

        private void syncLoadoutWithUI()
        {
            // Set all text elements to the slot names
            foreach (GameObject g in GUIloadoutDisplaySlots)
            {
                g.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().SetText(g.name);
            }

            // Overwrite all text elements that have a weapon in the corresponding loadout slot
            int count = 0;
            foreach (GameObject g in GUIloadoutDisplaySlots)
            {
                if (loadout[count] != null)
                {
                    g.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().SetText(loadout[count].name);
                }

                count++;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Resets the position and rotation of a gameObject
        /// </summary>
        /// <param name="g"></param>
        private void ResetTransform(GameObject g)
        {
            Debug.Log("Resetting transform of " + g.name);
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }
    }
}

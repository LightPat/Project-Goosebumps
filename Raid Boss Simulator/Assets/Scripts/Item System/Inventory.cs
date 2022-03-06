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

        [HideInInspector]
        public GameObject[] loadout = new GameObject[3];
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
            addWeaponServerRpc(g.GetComponent<NetworkObject>().NetworkObjectId, GetComponent<NetworkObject>().OwnerClientId);

            g.GetComponent<Rigidbody>().isKinematic = true;

            if (!IsHost)
            {
                g.transform.position = transform.Find("Vertical Rotate(Clone)").Find("Equipped Weapon Spawn Point(Clone)").position;
                g.transform.rotation = transform.Find("Vertical Rotate(Clone)").rotation;
            }

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

            g.SetActive(false);
        }

        [ServerRpc]
        void addWeaponServerRpc(ulong targetId, ulong clientId)
        {
            GameObject[] weapons = GameObject.FindGameObjectsWithTag("InventoryItem");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    g.transform.SetParent(GetComponent<PlayerController>().verticalRotate.Find("Equipped Weapon Spawn Point(Clone)"), false);

                    ResetTransform(g);

                    // If this is the host end here because we already executed all this code
                    if (IsClient) { return; }

                    g.GetComponent<Rigidbody>().isKinematic = true;

                    // For the clients I call updateCamera() when they equip their weapon
                    if (g.GetComponent<Weapon>())
                    {
                        g.GetComponent<Weapon>().updateCamera();
                    }

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

                    g.SetActive(false);

                    addWeaponClientRpc(targetId, clientId);
                }
            }
        }

        [ClientRpc]
        void addWeaponClientRpc(ulong targetId, ulong clientId)
        {
            if (IsLocalPlayer) { return; }

            GameObject[] weapons = GameObject.FindGameObjectsWithTag("InventoryItem");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    g.GetComponent<Rigidbody>().isKinematic = true;
                    g.transform.position = transform.Find("Vertical Rotate(Clone)").Find("Equipped Weapon Spawn Point(Clone)").position;
                    g.transform.rotation = transform.Find("Vertical Rotate(Clone)").rotation;

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

                    g.SetActive(false);
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
                        QueryLoadoutServerRpc(index);
                        // If this weapon is the same slot as we asked for, end so that we don't set active true again
                        if (g == loadout[index]) { return; }
                    }
                }
            }

            // At this point, there is no active equipped item, so we can set the queried weapon to active
            loadout[index].SetActive(true);
            loadout[index].GetComponent<Weapon>().updateCamera();
            QueryLoadoutServerRpc(index);
        }

        [ServerRpc]
        void QueryLoadoutServerRpc(int index)
        {
            // If this is the host end here since we already executed all this code
            if (IsClient) { return; }

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
                        QueryLoadoutClientRpc(index);
                        // If this weapon is the same slot as we asked for, end so that we don't set active true again
                        if (g == loadout[index]) { return; }
                    }
                }
            }

            // At this point, there is no active equipped item, so we can set the queried weapon to active
            loadout[index].SetActive(true);
            loadout[index].GetComponent<Weapon>().updateCamera();
            QueryLoadoutClientRpc(index);
        }

        [ClientRpc]
        void QueryLoadoutClientRpc(int index)
        {
            if (IsLocalPlayer) { return; }

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
            loadout[index].GetComponent<Weapon>().updateCamera();
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

                switchLoadoutSlotServerRpc(GetComponent<NetworkObject>().OwnerClientId, start, end);

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

        [ServerRpc]
        void switchLoadoutSlotServerRpc(ulong clientId, int start, int end)
        {
            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key == clientId)
                {
                    GameObject player = client.Value.PlayerObject.gameObject;

                    GameObject[] targetLoadout = player.GetComponent<Inventory>().loadout;

                    // Set every weapon to inactive
                    foreach (GameObject g in targetLoadout)
                    {
                        if (g != null)
                        {
                            g.SetActive(false);
                        }
                    }

                    // Call the weapon switch here
                    GameObject temp = loadout[end];
                    loadout[end] = loadout[start];
                    loadout[start] = temp;

                    switchLoadoutSlotClientRpc(clientId, start, end);
                }
            }
        }

        [ClientRpc]
        void switchLoadoutSlotClientRpc(ulong clientId, int start, int end)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players)
            {
                if (player.GetComponent<NetworkObject>().OwnerClientId == clientId & !player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    GameObject[] targetLoadout = player.GetComponent<Inventory>().loadout;

                    // Set every weapon to inactive
                    foreach (GameObject g in targetLoadout)
                    {
                        if (g != null)
                        {
                            g.SetActive(false);
                        }
                    }

                    // Call the weapon switch here
                    GameObject temp = loadout[end];
                    loadout[end] = loadout[start];
                    loadout[start] = temp;
                }
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
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }
    }
}

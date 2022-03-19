using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.Netcode;

namespace LightPat.Core.WeaponSystem
{
    public class WeaponLoadout : NetworkBehaviour
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
                GUIloadoutDisplaySlots[i] = GUICanvas.transform.Find("Slot " + (i+1).ToString()).gameObject;
            }
        }

        public void addItem(GameObject g)
        {
            if (g.transform.parent != null) { return; }

            GameObject reg = Instantiate(g.GetComponent<Weapon>().regularPrefab, GetComponent<PlayerController>().verticalRotate.Find("Equipped Weapon Spawn Point"), false);
            reg.name = g.GetComponent<Weapon>().regularPrefab.name;

            // Append gameobject to end of loadout if loadout slot is empty
            for (int i = 0; i < loadout.Length; i++)
            {
                if (loadout[i] == null)
                {
                    loadout[i] = reg;
                    HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(reg.name);
                    loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
                    reg.GetComponent<Weapon>().updateCamera();
                    reg.SetActive(false);
                    break;
                }
            }

            addWeaponServerRpc(g.GetComponent<NetworkObject>().NetworkObjectId);
        }

        [ServerRpc]
        void addWeaponServerRpc(ulong targetId)
        {
            addWeaponClientRpc(targetId);

            GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    if (IsHost)
                    {
                        g.GetComponent<NetworkObject>().Despawn();
                        return;
                    }

                    GameObject reg = Instantiate(g.GetComponent<Weapon>().regularPrefab, GetComponent<PlayerController>().verticalRotate.Find("Equipped Weapon Spawn Point"), false);
                    reg.name = g.GetComponent<Weapon>().regularPrefab.name;

                    // Append gameobject to end of loadout if loadout slot is empty
                    for (int i = 0; i < loadout.Length; i++)
                    {
                        if (loadout[i] == null)
                        {
                            loadout[i] = reg;
                            HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(reg.name);
                            loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
                            reg.GetComponent<Weapon>().updateCamera();
                            reg.SetActive(false);
                            break;
                        }
                    }

                    g.GetComponent<NetworkObject>().Despawn();
                }
            }
        }

        [ClientRpc]
        void addWeaponClientRpc(ulong targetId)
        {
            if (IsLocalPlayer) { return; }

            GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");

            foreach (GameObject g in weapons)
            {
                if (g.GetComponent<NetworkObject>().NetworkObjectId == targetId)
                {
                    GameObject reg = Instantiate(g.GetComponent<Weapon>().regularPrefab, GetComponent<PlayerController>().verticalRotate.Find("Equipped Weapon Spawn Point"), false);
                    reg.name = g.GetComponent<Weapon>().regularPrefab.name;

                    // Append gameobject to end of loadout if loadout slot is empty
                    for (int i = 0; i < loadout.Length; i++)
                    {
                        if (loadout[i] == null)
                        {
                            loadout[i] = reg;
                            HUDloadoutDisplaySlots[i].GetComponent<TextMeshProUGUI>().SetText(reg.name);
                            loadout[i].GetComponent<Weapon>().setTextDisplay(HUDloadoutDisplaySlots[i]);
                            reg.GetComponent<Weapon>().updateCamera();
                            reg.SetActive(false);
                            break;
                        }
                    }
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

            // If there is already an equipped weapon
            if (getEquippedWeapon() != null)
            {
                int equipIndex = getEquippedWeaponIndex();
                // Set the currently active equipped weapon to inactive, and return if that was the index we were trying to query
                getEquippedWeapon().SetActive(false);
                
                if (equipIndex == index)
                {
                    QueryLoadoutServerRpc(index); 
                    return;
                }
            }

            loadout[index].SetActive(true);
            QueryLoadoutServerRpc(index);
        }

        [ServerRpc]
        void QueryLoadoutServerRpc(int index)
        {
            // If this is the host end here since we already executed all this code
            if (IsClient) { return; }

            // If there is already an equipped weapon
            if (getEquippedWeapon() != null)
            {
                int equipIndex = getEquippedWeaponIndex();
                // Set the currently active equipped weapon to inactive, and return if that was the index we were trying to query
                getEquippedWeapon().SetActive(false);

                if (equipIndex == index)
                {
                    QueryLoadoutClientRpc(index);
                    return;
                }
            }

            loadout[index].SetActive(true);
            QueryLoadoutClientRpc(index);
        }

        [ClientRpc]
        void QueryLoadoutClientRpc(int index)
        {
            // Don't execute on client who initiated chain of calls
            if (IsLocalPlayer) { return; }

            // If there is already an equipped weapon
            if (getEquippedWeapon() != null)
            {
                int equipIndex = getEquippedWeaponIndex();
                // Set the currently active equipped weapon to inactive, and return if that was the index we were trying to query
                getEquippedWeapon().SetActive(false);

                if (equipIndex == index)
                {
                    return;
                }
            }

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
                selectedSlot = EventSystem.current.currentSelectedGameObject;
                originalColor = selectedSlot.GetComponent<Image>().color;
                selectedSlot.GetComponent<Image>().color = new Color32(0,0,0,100);
            }
            else
            {
                foreach (GameObject g in loadout)
                {
                    if (g != null)
                    {
                        g.SetActive(false);
                    }
                }

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
            if (IsClient) { return; }

            foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
            {
                if (client.Key == clientId)
                {
                    // Set every weapon to inactive
                    foreach (GameObject g in loadout)
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
                    // Set every weapon to inactive
                    foreach (GameObject g in loadout)
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

        // Dropping Weapons Logic

        void OnDrop()
        {
            if (getEquippedWeapon() == null) { return; }

            dropWeaponServerRpc(getEquippedWeaponIndex());
        }

        [ServerRpc]
        void dropWeaponServerRpc(int weaponIndex)
        {
            GameObject netWeapon = Instantiate(loadout[weaponIndex].GetComponent<Weapon>().networkedPrefab, loadout[weaponIndex].transform.position, loadout[weaponIndex].transform.rotation);
            netWeapon.GetComponent<NetworkObject>().Spawn();
            Destroy(loadout[weaponIndex]);

            Vector3 dropForce = netWeapon.transform.forward * 5;
            dropForce.y += 5;
            netWeapon.GetComponent<Rigidbody>().AddForce(dropForce, ForceMode.VelocityChange);

            // Apply random torque to the throw
            Vector3 torqueForce = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-10.0f, 10.0f), Random.Range(-0.1f, 0.1f));
            DisplayLogger.Instance.LogInfo(torqueForce.ToString());
            netWeapon.GetComponent<Rigidbody>().AddTorque(torqueForce, ForceMode.VelocityChange);

            dropWeaponClientRpc(weaponIndex);
        }

        [ClientRpc]
        void dropWeaponClientRpc(int weaponIndex)
        {
            Destroy(loadout[weaponIndex]);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Misc

        /// <summary>
        /// Resets the position and rotation of a gameObject
        /// </summary>
        /// <param name="g"></param>
        void ResetTransform(GameObject g)
        {
            g.transform.localPosition = Vector3.zero;
            g.transform.localRotation = Quaternion.identity;
        }
    }
}

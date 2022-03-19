using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using LightPat.Core;

public class PlayerVerticalRotate : NetworkBehaviour
{
    public GameObject WeaponSpawnPointPrefab;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        GameObject equippedWeaponSpawnPoint = Instantiate(WeaponSpawnPointPrefab);
        equippedWeaponSpawnPoint.GetComponent<NetworkObject>().SpawnWithOwnership(GetComponent<NetworkObject>().OwnerClientId);
        equippedWeaponSpawnPoint.transform.SetParent(transform, false);
    }
}

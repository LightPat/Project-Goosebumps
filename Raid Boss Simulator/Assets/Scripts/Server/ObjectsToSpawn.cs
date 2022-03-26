using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using LightPat.Core;

public class ObjectsToSpawn : NetworkBehaviour
{
    public GameObject[] objectsToSpawn;
    public Vector3[] spawnPositions;

    public void SpawnObjects()
    {
        if (!IsServer) { return; }

        if (spawnPositions.Length != objectsToSpawn.Length)
        {
            Debug.Log("You must associate each object with a spawn position. Check your server gameObject.");
            return;
        }

        for (int i = 0; i < objectsToSpawn.Length; i++)
        {
            GameObject spawned = Instantiate(objectsToSpawn[i], spawnPositions[i], Quaternion.identity);
            spawned.GetComponent<NetworkObject>().Spawn();
        }
    }
}
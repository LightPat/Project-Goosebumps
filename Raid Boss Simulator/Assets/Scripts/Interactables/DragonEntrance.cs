using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightPat.Core;
using Unity.Netcode;

namespace LightPat.Interactables
{
    public class DragonEntrance : Interactable
    {
        public GameObject Dragon;

        public override void Invoke()
        {
            GameObject dragon = Instantiate(Dragon, new Vector3(-108, -14, 0), Quaternion.Euler(0,90,0));
            dragon.GetComponent<NetworkObject>().Spawn();

            GetComponent<NetworkObject>().Despawn();
        }
    }
}
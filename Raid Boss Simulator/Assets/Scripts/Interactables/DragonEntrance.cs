using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DragonEntrance : Interactable
{
    public override void Invoke()
    {
        Destroy(gameObject);

    }
}

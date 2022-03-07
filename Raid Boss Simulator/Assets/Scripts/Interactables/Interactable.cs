using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public abstract class Interactable : NetworkBehaviour
{
    public abstract void Invoke();
}

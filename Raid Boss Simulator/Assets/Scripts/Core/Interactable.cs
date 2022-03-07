using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LightPat.Core
{
    public abstract class Interactable : NetworkBehaviour
    {
        public abstract void Invoke();
    }
}
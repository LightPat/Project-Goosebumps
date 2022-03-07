using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace LightPat.Mobs
{
    [RequireComponent(typeof(Rigidbody))]
    public class DragonController : NetworkBehaviour
    {
        private Rigidbody rb;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

namespace LightPat.Core
{
    public class Ping : NetworkBehaviour
    {
        public TextMeshProUGUI display;

        private int pingTime;
        public System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        private void Start()
        {
            if (IsOwner)
            {
                InvokeRepeating("startPing", 0f, 2f);
            }
        }

        void startPing()
        {
            watch.Start();
            pingServerRpc();
        }

        [ServerRpc]
        void pingServerRpc()
        {
            pingClientRpc();
        }

        [ClientRpc]
        void pingClientRpc()
        {
            display.SetText(watch.ElapsedMilliseconds.ToString() + " ms");
            watch.Reset();
        }
    }
}


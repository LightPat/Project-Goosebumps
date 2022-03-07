using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightPat.Core;

namespace LightPat.Interactables
{
    public class DestroySelf : Interactable
    {
        public override void Invoke()
        {
            Destroy(gameObject);
        }
    }
}
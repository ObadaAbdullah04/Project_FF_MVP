using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script will destroy particle game object after 1 second
    public class DestroyParticle : MonoBehaviour
    {
        void Start()
        {
            Destroy(this.gameObject, 1f);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script will destroy the are obstacle game object after 5 seconds
    public class DestroyTheAreaObstacle : MonoBehaviour
    {
        void Start()
        {
            Destroy(this.gameObject, 5f);
        }
    }
}

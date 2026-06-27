using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //Used to randomly spawn a star
    public class SpawnAStar : MonoBehaviour
    {
        void Start()
        {
            if (Random.Range(0, 10) != 0)
                this.gameObject.SetActive(false);

            transform.localPosition = new Vector2(Random.Range(5.75f, 6.75f), Random.Range(-5, 5));
        }
    }
}

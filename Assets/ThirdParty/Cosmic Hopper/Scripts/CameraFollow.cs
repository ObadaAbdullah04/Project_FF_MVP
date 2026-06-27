using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script is attached to the main camera and it is used to follow the player
    public class CameraFollow : MonoBehaviour
    {

        public GameObject player;
        private Vector3 velocity = Vector3.zero;

        void Update()
        {
            if (player == null) return;
            if (player.transform.position.x > transform.position.x - 2)
            {
                Vector3 targetPosition = new Vector3(player.transform.position.x + 2, transform.position.y, -10);
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.1f);
            }
        }

        public void FindThePlayer()
        {
            player = GameObject.Find("Player");
        }
    }
}

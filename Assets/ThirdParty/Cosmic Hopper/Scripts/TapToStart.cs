using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyJump
{
    //Used to start the game when player taps the screen
    public class TapToStart : MonoBehaviour
    {
        public void StartTheGame()
        {
            GameObject player = GameObject.Find("Player");
            player.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            player.GetComponent<PlayerMovement>().enabled = true;
            player.GetComponent<PlayerMovement>().Jump();
            this.gameObject.SetActive(false);
        }
    }
}

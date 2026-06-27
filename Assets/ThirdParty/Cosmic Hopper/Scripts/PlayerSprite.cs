using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script is attached to the player game object and it is used change the image to the one that is choosen in the shop menu
    public class PlayerSprite : MonoBehaviour
    {

        public Sprite[] playerSprite;

        void Start()
        {
            if (PlayerPrefs.GetInt("ChoosenItem", 0) == 0)
            {
                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                sprite.sprite = playerSprite[0];
            }
            else
            {
                int choosenItem = PlayerPrefs.GetInt("ChoosenItem", 0) - 1;
                SpriteRenderer sprite = GetComponent<SpriteRenderer>();
                sprite.sprite = playerSprite[choosenItem];
            }
        }
    }
}
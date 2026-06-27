using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyJump
{
    //Change the ball image to match the one that is selected in the shop menu
    public class TutorialBallImage : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] playerSprite;
        [SerializeField]
        private Image ballImage;

        void OnEnable()
        {
            if (PlayerPrefs.GetInt("ChoosenItem", 0) == 0)
            {
                ballImage.sprite = playerSprite[0];
            }
            else
            {
                int choosenItem = PlayerPrefs.GetInt("ChoosenItem", 0) - 1;
                ballImage.sprite = playerSprite[choosenItem];
            }
        }
    }
}

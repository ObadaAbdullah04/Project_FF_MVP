using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //Used to randomly generate colors on the ball game object
    public class PlayerColor : MonoBehaviour
    {
        public Color playerColor;
        void Start()
        {
            GetComponent<SpriteRenderer>().color = ChangePlayerColor();
        }

        public Color ChangePlayerColor()
        {
            int color = Random.Range(0, 4);

            if (color == 0)
            {
                ColorUtility.TryParseHtmlString("#A92DA3", out playerColor);
            }
            else if (color == 1)
            {
                ColorUtility.TryParseHtmlString("#00A1D3", out playerColor);
            }
            else if (color == 2)
            {
                ColorUtility.TryParseHtmlString("#FD4D2E", out playerColor);
            }
            else if (color == 3)
            {
                ColorUtility.TryParseHtmlString("#2DA971", out playerColor);
            }

            return playerColor;
        }
    }
}

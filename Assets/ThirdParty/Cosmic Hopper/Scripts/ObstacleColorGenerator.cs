using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //Used to randomly generate colors on the game areas
    public class ObstacleColorGenerator : MonoBehaviour
    {
        Color obstacleColor;

        void Start()
        {
            GenerateColor();
        }
        public void GenerateColor()
        {
            int numberOfChildGameObjects = transform.childCount;

            for (int i = 0; i < numberOfChildGameObjects; i++)
            {
                int color = Random.Range(0, 4);

                if (color == 0)
                {
                    ColorUtility.TryParseHtmlString("#A92DA3", out obstacleColor);
                }
                else if (color == 1)
                {
                    ColorUtility.TryParseHtmlString("#00A1D3", out obstacleColor);
                }
                else if (color == 2)
                {
                    ColorUtility.TryParseHtmlString("#FD4D2E", out obstacleColor);
                }
                else if (color == 3)
                {
                    ColorUtility.TryParseHtmlString("#2DA971", out obstacleColor);
                }

                if (i != 0 && obstacleColor == transform.GetChild(i - 1).GetComponent<SpriteRenderer>().color)//If generated color is the same as on previous game area
                {
                    i--;
                    continue;
                }

                transform.GetChild(i).GetComponent<SpriteRenderer>().color = obstacleColor;
            }

            bool isSameColor = false;

            Color playerColor = GameObject.Find("Player").GetComponent<PlayerColor>().playerColor;

            while (!isSameColor)
            {
                for (int i = 0; i < numberOfChildGameObjects; i++)
                {
                    if (transform.GetChild(i).GetComponent<SpriteRenderer>().color == playerColor)
                    {
                        isSameColor = true;
                        break;
                    }
                }

                if (isSameColor) break;

                int randomGameArea = Random.Range(0, numberOfChildGameObjects);
                if (randomGameArea == 0)
                {
                    if (transform.GetChild(1).GetComponent<SpriteRenderer>().color != playerColor)
                    {
                        transform.GetChild(0).GetComponent<SpriteRenderer>().color = playerColor;
                        isSameColor = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (transform.GetChild(randomGameArea - 1).GetComponent<SpriteRenderer>().color != playerColor)
                    {
                        transform.GetChild(randomGameArea).GetComponent<SpriteRenderer>().color = playerColor;
                        isSameColor = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

            }
        }
    }
}
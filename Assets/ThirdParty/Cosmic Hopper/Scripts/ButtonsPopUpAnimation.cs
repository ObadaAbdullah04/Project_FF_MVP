using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script is used to create a simple pop up animation
    public class ButtonsPopUpAnimation : MonoBehaviour
    {

        public float scale = 0;

        void OnEnable()
        {
            scale = 0;
        }

        void OnDisable()
        {
            transform.localScale = new Vector2(0, 0);
        }

        void Update()
        {
            if (scale < 1)
            {
                scale += Time.deltaTime * 3;
                transform.localScale = new Vector2(scale, scale);
                if (scale >= 1)
                {
                    transform.localScale = new Vector2(1, 1);
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This scirpt is used to create a simple animation on top and bottom borders
    public class BorderColorsAnimation : MonoBehaviour
    {
        [SerializeField]
        private GameObject borderColor1;
        [SerializeField]
        private GameObject borderColor2;
        public bool isReverse = false;
        private float speed = 5;

        // Update is called once per frame
        void Update()
        {
            if (isReverse)
            {
                borderColor1.transform.localPosition = new Vector3(borderColor1.transform.localPosition.x - Time.deltaTime * speed, borderColor1.transform.localPosition.y, 10);
                borderColor2.transform.localPosition = new Vector3(borderColor2.transform.localPosition.x - Time.deltaTime * speed, borderColor2.transform.localPosition.y, 10);

                if (borderColor1.transform.localPosition.x <= -14)
                {
                    borderColor1.transform.localPosition = new Vector3(14, borderColor1.transform.localPosition.y, 10);
                }

                if (borderColor2.transform.localPosition.x <= -14)
                {
                    borderColor2.transform.localPosition = new Vector3(14, borderColor2.transform.localPosition.y, 10);
                }
            }
            else
            {
                borderColor1.transform.localPosition = new Vector3(borderColor1.transform.localPosition.x + Time.deltaTime * speed, borderColor1.transform.localPosition.y, 10);
                borderColor2.transform.localPosition = new Vector3(borderColor2.transform.localPosition.x + Time.deltaTime * speed, borderColor2.transform.localPosition.y, 10);

                if (borderColor1.transform.localPosition.x >= 14)
                {
                    borderColor1.transform.localPosition = new Vector3(-14, borderColor1.transform.localPosition.y, 10);
                }

                if (borderColor2.transform.localPosition.x >= 14)
                {
                    borderColor2.transform.localPosition = new Vector3(-14, borderColor2.transform.localPosition.y, 10);
                }
            }

        }
    }
}

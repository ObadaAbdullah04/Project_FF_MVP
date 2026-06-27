using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //Static variables used throughout the game
    public class Vars : MonoBehaviour
    {
        public static int score = 0;
        public static float playerSpeed = 0; //Used inside PlayerMovement to increase the speed of the player each time the player scores
    }
}

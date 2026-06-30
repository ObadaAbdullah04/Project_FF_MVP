using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Templates.ColorCube
{


public class SoundSingleton : MonoBehaviour {
     public static SoundSingleton i;
     void Awake () {
         if(!i) {
             i = this;
             DontDestroyOnLoad(gameObject);
         }else 
                 Destroy(gameObject);
     }
}

}

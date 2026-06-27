using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //Game over logic when ball hits the border
    public class GameOver : MonoBehaviour
    {

        public GameObject explosion;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.name.Contains("Border"))
            {
                Project.MiniGames.UniversalGameController controller = Object.FindAnyObjectByType<Project.MiniGames.UniversalGameController>();
                if (controller != null && !controller.IsGameActive) return;

                if (controller != null) controller.ReportPlayerDeath();
                // GameObject.Find("GameManager").GetComponent<Menus>().GameOver();
                explosion.SetActive(true);
                if (this.transform.parent != null) explosion.transform.SetParent(this.transform.parent); else explosion.transform.parent = null;
                if (GameObject.Find("ExplosionSound") != null) GameObject.Find("ExplosionSound").GetComponent<AudioSource>().Play();
                Destroy(this.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FlappyJump
{
    //Player movement logic
    public class PlayerMovement : MonoBehaviour
    {

        public Rigidbody2D rb;
        private AudioSource jumpSound;
        private SpriteRenderer playerSprite;

        void Start()
        {
            jumpSound = GameObject.Find("JumpSound").GetComponent<AudioSource>();
            playerSprite = GetComponent<SpriteRenderer> ();
        }

        void FixedUpdate()
        {
            rb.velocity = new Vector2(4 + Vars.playerSpeed, rb.velocity.y);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Jump();
                InstantiateJumpParticle();
                jumpSound.Play();
            }
        }

        public void Jump()
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, 500));
            rb.angularVelocity = 0;
            rb.AddTorque(-10);
            // PlayerPrefs.SetInt("NumberOfJumps", PlayerPrefs.GetInt("NumberOfJumps") + 1);
        }

        private void InstantiateJumpParticle()
        {
            GameObject particle = Instantiate(Resources.Load("SuccessParticle")) as GameObject;
            if (particle != null) particle.transform.SetParent(this.transform.parent); // FIX LEAK

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            particle.transform.position = mousePos;
            
            var main = particle.GetComponent<ParticleSystem>().main;
            main.startColor = playerSprite.color;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.name == "Star")
            {
                // Architecture hook: Replace PlayerPrefs with ReportScore
                Project.MiniGames.UniversalGameController controller = Object.FindAnyObjectByType<Project.MiniGames.UniversalGameController>();
                if (controller != null) controller.ReportScore(1);

                GameObject starsParticle = Instantiate(Resources.Load("StarsParticle")) as GameObject;
                if (starsParticle != null) 
                {
                    starsParticle.transform.SetParent(this.transform.parent); // FIX LEAK
                    starsParticle.transform.position = new Vector3(col.gameObject.transform.position.x, col.gameObject.transform.position.y, 1);
                }
                Destroy(col.gameObject);
                GameObject.Find("StarSound")?.GetComponent<AudioSource>()?.Play();
            }
        }

    }
}

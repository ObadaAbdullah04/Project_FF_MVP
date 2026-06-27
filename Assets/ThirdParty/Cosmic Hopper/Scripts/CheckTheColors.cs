using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlappyJump
{
    //This script is used to check if the ball hit the right color
    public class CheckTheColors : MonoBehaviour
    {
        public GameObject explosion;
        AudioSource successSound;
        PlayerColor playerColor;

        void Start()
        {
            successSound = GameObject.Find("SuccessSound")?.GetComponent<AudioSource>();
            playerColor = GetComponent<PlayerColor>();
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.name.Equals("Star")) return;

            Project.MiniGames.UniversalGameController controller = Object.FindAnyObjectByType<Project.MiniGames.UniversalGameController>();

            // Architecture hook: Ignore physics after win condition
            if (controller != null && !controller.IsGameActive) return;

            if (GetComponent<SpriteRenderer>().color == col.GetComponent<SpriteRenderer>().color)
            {
                Vars.score++;
                
                GameObject newArea = Instantiate(Resources.Load("AreaObstacles")) as GameObject;
                newArea.transform.position = new Vector2(col.transform.parent.parent.position.x + 12.7f, 0);
                newArea.name = "AreaObstacles";
                newArea.transform.SetParent(this.transform.parent); // Required for scene unloading

                if (Vars.score < 5)
                {
                    newArea.transform.Find("ColorAreas3").name = "ColorAreas";
                    newArea.transform.Find("ColorAreas4").gameObject.SetActive(false);
                    newArea.transform.Find("ColorAreas5").gameObject.SetActive(false);
                }
                else if (Vars.score >= 5 && Vars.score < 10)
                {
                    newArea.transform.Find("ColorAreas3").gameObject.SetActive(false);
                    newArea.transform.Find("ColorAreas4").name = "ColorAreas";
                    newArea.transform.Find("ColorAreas5").gameObject.SetActive(false);
                }
                else
                {
                    newArea.transform.Find("ColorAreas3").gameObject.SetActive(false);
                    newArea.transform.Find("ColorAreas4").gameObject.SetActive(false);
                    newArea.transform.Find("ColorAreas5").name = "ColorAreas";
                }
                newArea.transform.Find("ColorAreas").GetComponent<ObstacleColorGenerator>().GenerateColor();

                GameObject.Find("Player").GetComponent<PlayerMovement>().enabled = true;
                Vars.playerSpeed += 0.05f;

                if (col.transform.parent.parent.gameObject.GetComponent<DestroyTheAreaObstacle>() != null)
                    col.transform.parent.parent.gameObject.GetComponent<DestroyTheAreaObstacle>().enabled = true;
                Vars.playerSpeed += 0.05f;

                // Architecture hook: Report Score instead of setting PlayerPrefs
                if (controller != null) controller.ReportScore(1);

                GameObject successParticle = Instantiate(Resources.Load("SuccessParticle")) as GameObject;
                successParticle.transform.SetParent(this.transform.parent); // Required for scene unloading

                ParticleSystem ps = successParticle.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = GetComponent<SpriteRenderer>().color;
                successParticle.transform.position = col.ClosestPoint(transform.position);
                successParticle.name = "SuccessParticle";
                StartCoroutine(ChangeColor(GetComponent<SpriteRenderer>(), playerColor.ChangePlayerColor()));

                successSound?.Play();
            }
            else
            {
                // Architecture hook: Report Death instead of Menus.GameOver()
                if (controller != null) controller.ReportPlayerDeath();
                
                explosion.SetActive(true);
                if (this.transform.parent != null) explosion.transform.SetParent(this.transform.parent); else explosion.transform.parent = null;
                
                ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = GetComponent<SpriteRenderer>().color;
                GameObject.Find("ExplosionSound")?.GetComponent<AudioSource>()?.Play();
                Destroy(this.gameObject);
            }
        }

        IEnumerator ChangeColor(SpriteRenderer sp, Color c) //This is used for smooth color transition
        {
            float time = 0;
            while (time < 1.2f)
            {
                time += Time.deltaTime;

                if (sp == null)
                {
                    time = 1.2f;
                    yield return null;
                }

                sp.color = Color.Lerp(sp.color, c, Time.deltaTime * 10);

                if (time > 1) sp.color = c;
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
    }
}
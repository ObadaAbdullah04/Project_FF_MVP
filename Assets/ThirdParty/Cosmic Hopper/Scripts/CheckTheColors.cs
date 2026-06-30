using System.Collections;
using UnityEngine;

namespace FlappyJump
{
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

            CosmicHopperAdapter adapter = CosmicHopperAdapter.Instance;

            if (GetComponent<SpriteRenderer>().color == col.GetComponent<SpriteRenderer>().color)
            {
                adapter.OnScoreChanged();
                adapter.playerSpeed += 0.05f;

                GameObject newArea = Instantiate(Resources.Load("AreaObstacles")) as GameObject;
                newArea.transform.position = new Vector2(col.transform.parent.parent.position.x + 12.7f, 0);
                newArea.name = "AreaObstacles";
                newArea.transform.SetParent(this.transform.parent);

                if (adapter.Score < 5)
                {
                    newArea.transform.Find("ColorAreas3").name = "ColorAreas";
                    newArea.transform.Find("ColorAreas4").gameObject.SetActive(false);
                    newArea.transform.Find("ColorAreas5").gameObject.SetActive(false);
                }
                else if (adapter.Score >= 5 && adapter.Score < 10)
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

                if (col.transform.parent.parent.gameObject.GetComponent<DestroyTheAreaObstacle>() != null)
                    col.transform.parent.parent.gameObject.GetComponent<DestroyTheAreaObstacle>().enabled = true;

                GameObject successParticle = Instantiate(Resources.Load("SuccessParticle")) as GameObject;
                successParticle.transform.SetParent(this.transform.parent);

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
                adapter.OnPlayerDeath();

                explosion.SetActive(true);
                if (this.transform.parent != null) explosion.transform.SetParent(this.transform.parent); else explosion.transform.parent = null;

                ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.startColor = GetComponent<SpriteRenderer>().color;
                GameObject.Find("ExplosionSound")?.GetComponent<AudioSource>()?.Play();
                Destroy(this.gameObject);
            }
        }

        IEnumerator ChangeColor(SpriteRenderer sp, Color c)
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

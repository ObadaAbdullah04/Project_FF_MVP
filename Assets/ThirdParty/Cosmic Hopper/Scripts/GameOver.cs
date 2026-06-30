using UnityEngine;

namespace FlappyJump
{
    public class GameOver : MonoBehaviour
    {
        public GameObject explosion;

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.name.Contains("Border"))
            {
                CosmicHopperAdapter adapter = CosmicHopperAdapter.Instance;
                if (adapter != null) adapter.OnPlayerDeath();

                explosion.SetActive(true);
                if (this.transform.parent != null) explosion.transform.SetParent(this.transform.parent); else explosion.transform.parent = null;
                if (GameObject.Find("ExplosionSound") != null) GameObject.Find("ExplosionSound").GetComponent<AudioSource>().Play();
                Destroy(this.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Templates.ColorCube
{


public class CubeCollisionDetection : MonoBehaviour {

	public GameObject explosion;

	void OnCollisionEnter(Collision collision)
	{
		GameOver();
		Destroy(collision.gameObject);
	}

	void OnTriggerEnter(Collider col) {
		if(GetComponent<BoxCollider>().isTrigger && col.gameObject.GetComponent<BoxCollider>().isTrigger && !col.gameObject.name.Equals("success")) {
			GameOver();
			Destroy(col.transform.parent.gameObject);
		}
	}

	private void GameOver() {
		if (ColorCubeAdapter.Instance != null)
			ColorCubeAdapter.Instance.OnPlayerDeath();

		GameObject.Find("explosionSound")?.GetComponent<AudioSource> ()?.Play();
		explosion.transform.parent = null;
		explosion.SetActive(true);
		Camera.main.transform.parent = null;
		Destroy(transform.parent.gameObject);
	}
}

}

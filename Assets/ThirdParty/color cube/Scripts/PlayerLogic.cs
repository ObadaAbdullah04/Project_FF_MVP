using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Templates.ColorCube
{


public class PlayerLogic : MonoBehaviour {

	private Text scoreText;
	public float speed = 0.3f;
	public Rigidbody rb;
	private AudioSource successSound;
	private int score = 0;
	public static int collision = 0;

	void Start() {
		successSound = GameObject.Find("successSound")?.GetComponent<AudioSource> ();
		GameObject canvas = GameObject.Find("Canvas");
		if (canvas != null)
		{
			Transform gp = canvas.transform.Find("gameplayUI");
			if (gp != null) scoreText = gp.Find("score")?.GetComponent<Text>();
		}
		score = PlayerPrefs.GetInt("lastScore", 0);
		if (scoreText != null) scoreText.text = "SCORE: " + score;
		

		if(collision >= 1) {//this is used to check if player crashed and than continued game after watching the ad
			GameObject newObstacle = Instantiate (Resources.Load ("obstacle") as GameObject);
			newObstacle.transform.parent = GameObject.Find("obstacles").transform;
			newObstacle.transform.localPosition = new Vector3(0,0.55f, (score + collision) * 100 + 100);

			GameObject newFloor = Instantiate (Resources.Load ("floor") as GameObject);
			newFloor.transform.parent = GameObject.Find("floorPlanes").transform;
			newFloor.transform.localPosition = new Vector3(0,0.55f, (score + collision) * 100);
			speed = 0.3f + ((float)score / 100);
		}
	}

	void FixedUpdate () {
		rb.transform.position = new Vector3 (rb.transform.position.x, rb.transform.position.y, rb.transform.position.z + speed);
	}
	void OnTriggerEnter(Collider col) {
		if(col.gameObject.name.Equals("success")){
			score++;
			PlayerPrefs.SetInt("lastScore", score);
			if(score > PlayerPrefs.GetInt("bestScore", 1)){
				PlayerPrefs.SetInt("bestScore", score);
			}
			if (scoreText != null) scoreText.text = "SCORE: " + score;
			GameObject newObstacle = Instantiate (Resources.Load ("obstacle") as GameObject);
			newObstacle.transform.parent = GameObject.Find("obstacles").transform;
			newObstacle.transform.localPosition = new Vector3(0,0.55f, (score + collision) * 100 + 100);

			Destroy(col.transform.parent.gameObject, 0.5f);

			GameObject newFloor = Instantiate (Resources.Load ("floor") as GameObject);
			newFloor.transform.parent = GameObject.Find("floorPlanes").transform;
			newFloor.transform.localPosition = new Vector3(0,0.55f, (score + collision) * 100);

			GameObject[] floorGameObjects = GameObject.FindGameObjectsWithTag("floor");
			foreach (GameObject floor in floorGameObjects)
			{
				if(floor.transform.position.z < (transform.position.z - 100)) {
					Destroy(floor);
				}
			}

			if (successSound != null) successSound.Play();
			speed+=0.01f;

			if (ColorCubeAdapter.Instance != null)
				ColorCubeAdapter.Instance.OnScoreChanged();
		}
	}
	
}

}

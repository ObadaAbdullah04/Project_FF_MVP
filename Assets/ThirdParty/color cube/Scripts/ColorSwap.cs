using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Templates.ColorCube
{


public class ColorSwap : MonoBehaviour {

	private Color color;
	private AudioSource changeSound;

	void Start() {
		color = new Color (0, 0.7f, 1, 1);
		changeSound = GameObject.Find("colorChangeSound").GetComponent<AudioSource> ();
		Renderer[] kocke = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < kocke.Length; i++) {
			kocke[i].material.SetColor ("_Color", color);
			kocke[i].material.SetColor ("_BaseColor", color);
		}
	}

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) { 
				if (hit.collider.tag.Equals ("box")) {
					PlayerPrefs.SetInt("colorChange", PlayerPrefs.GetInt("colorChange") + 1);
					changeSound.Play();
					Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
					Color current = rend.material.GetColor ("_BaseColor");
					if (current.Equals(color)) {
						hit.collider.isTrigger = true;
						Color white = new Color (1, 1, 1, 0.7f);
						rend.material.SetColor ("_Color", white);
						rend.material.SetColor ("_BaseColor", white);
					} else {
						hit.collider.isTrigger = false;
						rend.material.SetColor ("_Color", color);
						rend.material.SetColor ("_BaseColor", color);
					}
				}
			}
		}
	}
		
}

}

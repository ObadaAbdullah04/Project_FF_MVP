using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Templates.ColorCube
{


public class Obstacle : MonoBehaviour {

	void Start () {
		Renderer[] renderer = GetComponentsInChildren<Renderer> ();
		BoxCollider[] cubeCollider = GetComponentsInChildren<BoxCollider> ();
		Color blue = new Color (0, 0.7f, 1, 1);
		Color white = new Color (1, 1, 1, 0.7f);
		for (int i = 0; i < renderer.Length; i++) {
			int rand = Random.Range (0, 2);
			Material mat = renderer [i].material;
			if (rand == 0) {
				mat.SetColor ("_Color", blue);
				mat.SetColor ("_BaseColor", blue);
				cubeCollider [i].isTrigger = true;
			} else {
				mat.SetColor ("_Color", white);
				mat.SetColor ("_BaseColor", white);
				cubeCollider [i].isTrigger = false;
			}
		}
	}
}

}

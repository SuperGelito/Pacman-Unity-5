using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D co) {
		// Do Stuff...
		if (co.name == "Pacman")
			Destroy(gameObject);
		//increase points
	}
}

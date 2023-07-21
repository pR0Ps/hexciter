using UnityEngine;
using System.Collections;

public class MenuParticles : MonoBehaviour {
	
	void Start () {
		float t = Random.Range (10f, GetComponent<ParticleSystem>().main.duration);
		GetComponent<ParticleSystem>().Simulate (t);
		GetComponent<ParticleSystem>().Play ();
	}
}

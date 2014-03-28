using UnityEngine;
using System.Collections;

public class MenuParticles : MonoBehaviour {
	
	void Start () {
		float t = Random.Range (10f, particleSystem.duration);
		particleSystem.Simulate (t);
		particleSystem.Play ();
	}
}

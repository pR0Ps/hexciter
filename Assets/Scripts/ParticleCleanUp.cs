using UnityEngine;
using System.Collections;

public class ParticleCleanUp : MonoBehaviour {
	void Update () {
		if (!GetComponent<ParticleSystem>().IsAlive()) {
			gameObject.SetActive(false);
			ObjectPoolManager.Instance.Push(gameObject);
		}
	}
}

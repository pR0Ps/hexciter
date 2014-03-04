using UnityEngine;
using System.Collections;

public class ParticleCleanUp : MonoBehaviour {
	void Update () {
		if (!particleSystem.IsAlive()) {
			gameObject.SetActive(false);
			ObjectPoolManager.Instance.Push(gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class ScorePopup : MonoBehaviour {

	TextMesh tm;
	Animation anim;

	void Awake () {
		tm = GetComponentInChildren<TextMesh>();
		anim = GetComponentInChildren<Animation>();
	}

	// change the text mesh to show the score, move the object and start the animation
	public void Show (int score, Vector3 position) {
		Show(score.ToString(), position);
	}

	public void Show (string text, Vector3 position) {
		gameObject.SetActive (true);
		tm.text = text;
		anim.Play("popup");
		transform.position = position;
	}

	// Push this back to the object pool if it's done its animation
	void Update () {
		if (!anim.isPlaying)
			ObjectPoolManager.Instance.Push(gameObject);
	}
}

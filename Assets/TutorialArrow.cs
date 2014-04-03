using UnityEngine;
using System.Collections;

public class TutorialArrow : MonoBehaviour {

	SpriteRenderer sr;

	void Awake () {
		sr = GetComponentInChildren<SpriteRenderer> ();
	}

	public void FadeIn () {
		StartCoroutine (IFadeIn ());
	}

	IEnumerator IFadeIn () {
		float t = 0;
		while (t < 1) {
			t = Mathf.Min(1, t + Time.deltaTime * 4);
			sr.color = new Color(1,1,1,t);
			yield return new WaitForEndOfFrame();
		}
	}

	public void FadeOut () {
		StartCoroutine (IFadeOut ());
	}
	
	IEnumerator IFadeOut () {
		float t = 1;
		while (t > 0) {
			t = Mathf.Min(1, t - Time.deltaTime * 4);
			sr.color = new Color(1,1,1,t);
			yield return new WaitForEndOfFrame();
		}
	}
}

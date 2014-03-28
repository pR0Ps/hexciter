using UnityEngine;
using System;
using System.Collections;

public class FadeCam : MonoBehaviour {

	private static FadeCam _instance;
	GameObject whiteplane;
	bool busy;

	public static FadeCam Instance {
		get {
			if (!_instance) {
				_instance = GameObject.Find("FadeCam").GetComponent<FadeCam>();
			}
			return _instance;
		}
		private set { }
	}

	void Awake () {
		whiteplane = transform.FindChild ("whiteSprite").gameObject;
		whiteplane.SetActive (false);
	}

	public void FadeIn () {
		FadeIn(() => { }); // pass an empty lambda if no callback is specified
	}

	public void FadeIn (Action callback) {
		if (busy) return;
		StartCoroutine (IFadeIn(callback));
	}

	IEnumerator IFadeIn (Action callback) {
		busy = true;
		whiteplane.SetActive (true);
		animation.Play ("fadein");
		yield return new WaitForSeconds (0.5f);
		callback.Invoke ();
		whiteplane.SetActive (false);
		busy = false;
	}

	public void FadeOut () {
		FadeOut(() => { }); // pass an empty lambda if no callback is specified
	}
	
	public void FadeOut (Action callback) {
		if (busy) return;
		StartCoroutine (IFadeOut(callback));
	}
	
	IEnumerator IFadeOut (Action callback) {
		busy = true;
		whiteplane.SetActive (true);
		animation.Play ("fadeout");
		yield return new WaitForSeconds (0.5f);
		callback.Invoke ();
		busy = false;
	}
}

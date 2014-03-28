using UnityEngine;
using System;
using System.Collections;

public class FadeCam : MonoBehaviour {

	private static FadeCam _instance;
	GameObject whiteplane;

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
		StartCoroutine (IFadeIn(() => { })); // pass an empty lambda if no callback is specified
	}

	public void FadeIn (Action callback) {
		StartCoroutine (IFadeIn(callback));
	}

	IEnumerator IFadeIn (Action callback) {
		whiteplane.SetActive (true);
		animation.Play ("fadein");
		yield return new WaitForSeconds (0.5f);
		callback.Invoke ();
		whiteplane.SetActive (false);
	}

	public void FadeOut () {
		StartCoroutine (IFadeOut(() => { })); // pass an empty lambda if no callback is specified
	}
	
	public void FadeOut (Action callback) {
		StartCoroutine (IFadeOut(callback));
	}
	
	IEnumerator IFadeOut (Action callback) {
		whiteplane.SetActive (true);
		animation.Play ("fadeout");
		yield return new WaitForSeconds (0.5f);
		callback.Invoke ();
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameoverDialog : InteractiveObject {

	TextMesh[] texts;
	TextMesh score;
	TextMesh best;
	SpriteRenderer sr;
	float alpha;

	void Awake () {
		texts = GetComponentsInChildren<TextMesh> ();
		score = transform.Find ("score").GetComponent<TextMesh> ();
		best = transform.Find ("best").GetComponent<TextMesh> ();
		sr = GetComponentInChildren<SpriteRenderer> ();
		alpha = sr.color.a;
		gameObject.SetActive (false);
	}

	public void Show (int s, int b) {
		gameObject.SetActive (true);
		GetComponent<Animation>().Play ("gameoverShow");
		score.text = s.ToString ("N0");
		best.text = b.ToString ("N0");
	}

	public override void DownAction () {
		FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("game");});
	}

	void Update () {
		if (sr.color.a != alpha) {
			alpha = sr.color.a;
			for (int i=0; i<texts.Length; i++) {
				texts [i].color = new Color (0, 0, 0, alpha);
			}
		}
	}
}

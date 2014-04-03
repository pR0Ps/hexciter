using UnityEngine;
using System.Collections;

public class TutorialText : MonoBehaviour {

	TextMesh tm;
	TextMesh subText;

	void Awake () {
		tm = GetComponent<TextMesh> ();
		subText = transform.FindChild ("subText").GetComponent<TextMesh> ();
	}

	public IEnumerator Show (string s, string sub) {
		tm.text = s;
		subText.text = sub;
		animation.Play ("tuttextshow");
		while (animation ["tuttextshow"].normalizedTime < 1) {
			tm.color = new Color (0, 0, 0, animation ["tuttextshow"].normalizedTime);
			subText.color = tm.color;
			yield return new WaitForEndOfFrame();
		}
		tm.color = new Color (0, 0, 0, 1);
		subText.color = tm.color;
	}

	public IEnumerator Hide () {
		animation.Play ("tuttexthide");
		while (animation ["tuttexthide"].normalizedTime < 1) {
			tm.color = new Color (0, 0, 0, 1 - animation ["tuttexthide"].normalizedTime);
			subText.color = tm.color;
			yield return new WaitForEndOfFrame();
		}
		tm.color = new Color (0, 0, 0, 0);
		subText.color = tm.color;
	}
}

using UnityEngine;
using System.Collections;

public class TutorialText : MonoBehaviour {

	TextMesh tm;
	TextMesh subText;

	void Awake () {
		tm = GetComponent<TextMesh> ();
		subText = transform.Find ("subText").GetComponent<TextMesh> ();
	}

	public IEnumerator Show (string s, string sub) {
		tm.text = s;
		subText.text = sub;
		GetComponent<Animation>().Play ("tuttextshow");
		while (GetComponent<Animation>() ["tuttextshow"].normalizedTime < 1) {
			tm.color = new Color (0, 0, 0, GetComponent<Animation>() ["tuttextshow"].normalizedTime);
			subText.color = tm.color;
			yield return new WaitForEndOfFrame();
		}
		tm.color = new Color (0, 0, 0, 1);
		subText.color = tm.color;
	}

	public IEnumerator Hide () {
		GetComponent<Animation>().Play ("tuttexthide");
		while (GetComponent<Animation>() ["tuttexthide"].normalizedTime < 1) {
			tm.color = new Color (0, 0, 0, 1 - GetComponent<Animation>() ["tuttexthide"].normalizedTime);
			subText.color = tm.color;
			yield return new WaitForEndOfFrame();
		}
		tm.color = new Color (0, 0, 0, 0);
		subText.color = tm.color;
	}
}

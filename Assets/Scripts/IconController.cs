using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour {

	IconAnimation paintIcon;
	IconAnimation cubeIcon;

	void Awake () {
		paintIcon = transform.Find ("PaintBucket").GetComponent<IconAnimation> ();
		cubeIcon = transform.Find ("CubeExplode").GetComponent<IconAnimation> ();
	}

	public void SetInvisible () {
		paintIcon.SetVisiblity (0);
		cubeIcon.SetVisiblity (0);
	}

	public void SetTarget (float t) {
		paintIcon.SetVisiblity (Mathf.Clamp((t/2) + 0.5f,0, 1));
		cubeIcon.SetVisiblity (Mathf.Clamp((-t/2) + 0.5f,0, 1));
	}
}

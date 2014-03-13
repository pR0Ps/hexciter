using UnityEngine;
using System.Collections;

public class IconController : MonoBehaviour {

	static public IconController Instance { get; private set; }
	IconAnimation paintIcon;
	IconAnimation cubeIcon;

	void Awake () {
		Instance = this;
		paintIcon = transform.FindChild ("PaintBucket").GetComponent<IconAnimation> ();
		cubeIcon = transform.FindChild ("CubeExplode").GetComponent<IconAnimation> ();
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

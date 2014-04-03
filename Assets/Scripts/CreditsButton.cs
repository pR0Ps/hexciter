using UnityEngine;
using System.Collections;

public class CreditsButton : InteractiveObject {
	
	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {Application.LoadLevel("credits");}))
			animation.Play ("buttonpress");
	}
}

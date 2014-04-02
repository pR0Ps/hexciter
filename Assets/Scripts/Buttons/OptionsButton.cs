using UnityEngine;
using System.Collections;

public class OptionsButton : InteractiveObject {

	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {Application.LoadLevel("options");}))
			animation.Play ("buttonpress");
	}
}

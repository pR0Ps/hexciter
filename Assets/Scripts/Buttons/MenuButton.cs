using UnityEngine;
using System.Collections;

public class MenuButton : InteractiveObject {
	
	public override void TapAction () {
		if (FadeCam.Instance.FadeOut(() => {Application.LoadLevel("menu");}))
			animation.Play ("buttonpress");
	}
}
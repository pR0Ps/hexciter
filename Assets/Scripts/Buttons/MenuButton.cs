using UnityEngine;
using System.Collections;

public class MenuButton : InteractiveObject {
	
	public override void TapAction () {
		FadeCam.Instance.FadeOut(() => {Application.LoadLevel("menu");});
		animation.Play ("buttonpress");
	}
}
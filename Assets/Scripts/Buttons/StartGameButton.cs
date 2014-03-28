using UnityEngine;
using System.Collections;

public class StartGameButton : InteractiveObject {

	public override void DownAction () {
		FadeCam.Instance.FadeOut(() => {Application.LoadLevel("game");});
		animation.Play ("buttonpress");
	}
}

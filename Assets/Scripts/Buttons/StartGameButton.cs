using UnityEngine;
using System.Collections;

public class StartGameButton : InteractiveObject {

	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {Application.LoadLevel("game");}))
			animation.Play ("buttonpress");
	}
}

using UnityEngine;
using System.Collections;

public class StartTutorialButton :  InteractiveObject {
	
	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {Application.LoadLevel("tutorial");}))
			animation.Play ("buttonpress");
	}
}
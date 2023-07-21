using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OptionsButton : InteractiveObject {

	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("options");}))
			GetComponent<Animation>().Play ("buttonpress");
	}
}

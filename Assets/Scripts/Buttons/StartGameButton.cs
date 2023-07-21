using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGameButton : InteractiveObject {

	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("game");}))
			GetComponent<Animation>().Play ("buttonpress");
	}
}

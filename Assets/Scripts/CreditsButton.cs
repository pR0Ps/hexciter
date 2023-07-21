using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CreditsButton : InteractiveObject {
	
	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("credits");}))
			GetComponent<Animation>().Play ("buttonpress");
	}
}

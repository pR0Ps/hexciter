using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartTutorialButton :  InteractiveObject {
	
	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("tutorial");}))
			GetComponent<Animation>().Play ("buttonpress");
	}
}
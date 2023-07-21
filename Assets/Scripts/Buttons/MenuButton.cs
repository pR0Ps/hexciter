using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButton : InteractiveObject {
	
	public override void DownAction () {
		if (FadeCam.Instance.FadeOut(() => {SceneManager.LoadScene("menu");}))
			GetComponent<Animation>().Play ("buttonpress");
	}
}
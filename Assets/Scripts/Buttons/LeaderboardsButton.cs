using UnityEngine;
using System.Collections;

public class LeaderboardsButton : InteractiveObject {

	public GameObject socialButton;
	private bool busy;

	public void Awake(){
		#if (!UNITY_ANDROID && !UNITY_IPHONE)
			gameObject.SetActive(false);
		#elif UNITY_ANDROID
			socialButton.transform.Find("gpgicon").gameObject.SetActive(true);
		#elif UNITY_IPHONE
			socialButton.transform.Find("gcicon").gameObject.SetActive(true);
		#endif
		busy = false;
	}

	IEnumerator ShowHighscores(){

		try{
			if (!Social.localUser.authenticated){
				SocialManager.Instance.Login();

				while (SocialManager.Instance.busy)
					yield return new WaitForEndOfFrame();
			}

			if (Social.localUser.authenticated){
				SocialManager.Instance.ShowLeaderboards("highscores");
			}
		}
		finally{
			busy = false;
		}
	}
	
	public override void DownAction () {
		if (busy)
			return;

		busy = true;
		animation.Play ("buttonpress");
		socialButton.animation.Play("ShakeButton");

		StartCoroutine(ShowHighscores());
	}
}

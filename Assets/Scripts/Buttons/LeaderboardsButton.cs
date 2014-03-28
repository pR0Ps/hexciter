using UnityEngine;
using System.Collections;

public class LeaderboardsButton : InteractiveObject {

	public SocialButton socialButton;

	public void Awake(){
		#if (!UNITY_ANDROID && !UNITY_IPHONE)
			gameObject.SetActive(false);
		#endif
	}
	
	public override void DownAction () {
		animation.Play ("buttonpress");
		if (Social.localUser.authenticated){
			#if UNITY_ANDROID
			((GooglePlayGames.PlayGamesPlatform)Social.Active).ShowLeaderboardUI("30-move");
			#elif UNITY_IPHONE
			Social.ShowLeaderboardUI();
			#endif
		}
		else{
			Debug.Log("Not logged in, can't show leaderboards");
			if (socialButton != null) socialButton.Shake();
		}
	}
}

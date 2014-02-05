using UnityEngine;
using System.Collections;

public class LeaderboardsButton : InteractiveObject {
	
	public override void TapAction () {
		if (Social.localUser.authenticated){
			#if UNITY_ANDROID
			((GooglePlayGames.PlayGamesPlatform)Social.Active).ShowLeaderboardUI("30-move");
			#else
			Social.ShowLeaderboardUI();
			#endif
		}
		else{
			Debug.Log("Not logged in, can't show leaderboards");
		}
	}
}

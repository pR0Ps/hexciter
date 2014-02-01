using UnityEngine;
using System.Collections;

public class GPGManager: MonoBehaviour {

	//Only flesh out this class if we are on a platform that supports GPG
	#if (UNITY_IPHONE || UNITY_ANDROID)

	//Activate the play games platform
	void Awake(){
		Debug.Log ("Activating GPG");
		GooglePlayGames.PlayGamesPlatform.Activate();
		Login ();
	}
	
	void Login () {
		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate((bool success) => {
				if(success){
					Debug.Log("Logged in!");

				}
				else{
					Debug.Log("Couldn't log in");
				}
			});
		}
	}

	void Logout(){
		if (Social.localUser.authenticated) {
			((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
		}
		else{
			Debug.Log("Already logged out");
		}
	}

	void PostScore(int score){
		Social.ReportScore(score, "CgkIucKVsZ8TEAIQAg", (bool success) => {
			if(success){
				Debug.Log("Posted score!");
			}
			else{
				Debug.Log("Couldn't post score");
			}
		});
	}

	void UnlockAchievement(){
		Social.ReportProgress("CgkIucKVsZ8TEAIQAA", 100.0f, (bool success) => {
			if(success){
				Debug.Log("Unlocked achievement!");
			}
			else{
				Debug.Log("Couldn't unlock achievement");
			}
		});
	}

	void IncrementAchievement(){
		((GooglePlayGames.PlayGamesPlatform) Social.Active).IncrementAchievement(
			"CgkIucKVsZ8TEAIQAQ", 1, (bool success) => {
			if(success){
				Debug.Log("Incremented achievement by 1!");
			}
			else{
				Debug.Log("Couldn't increment achievement");
			}
		});
	}

	void ShowAchievements(){
		Social.ShowAchievementsUI();
	}

	void ShowLeaderboards(){
		Social.ShowLeaderboardUI();
	}

	#endif
}

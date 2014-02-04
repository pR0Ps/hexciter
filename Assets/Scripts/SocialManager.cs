﻿using UnityEngine;
using System.Collections;

public class SocialManager: MonoBehaviour {

	public static SocialManager Instance {get; private set;} // Singleton Instance

	//Only flesh out this class if we are on a platform that supports GPG
	#if (UNITY_IPHONE || UNITY_ANDROID)

	//Activate the play games platform
	void Awake(){
		Instance = this;
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

	public void Logout(){
		if (Social.localUser.authenticated) {
			((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
		}
		else{
			Debug.Log("Already logged out");
		}
	}

	public void PostScore(int score){
		Debug.Log("Attempting to post score");
		Social.ReportScore(score, "CgkIucKVsZ8TEAIQAg", (bool success) => {
			if(success){
				Debug.Log("Posted score!");
			}
			else{
				Debug.Log("Couldn't post score");
			}
		});
	}

	public void UnlockAchievement(){
		Social.ReportProgress("CgkIucKVsZ8TEAIQAA", 100.0f, (bool success) => {
			if(success){
				Debug.Log("Unlocked achievement!");
			}
			else{
				Debug.Log("Couldn't unlock achievement");
			}
		});
	}

	public void IncrementAchievement(){
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

	public void ShowAchievements(){
		Social.ShowAchievementsUI();
	}

	public void ShowLeaderboards(){
		Debug.Log("Attempting show scores");
		Social.ShowLeaderboardUI();
	}

	#endif
}
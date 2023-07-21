using UnityEngine;
using System.Collections;
using System;

public class SocialManager: MonoBehaviour {

	public bool busy {get; private set;}

	#if (UNITY_IPHONE || UNITY_ANDROID)
	private static string[] NAMES = {"hexciting", "total annihilation", "everyone wins", "hello world",
									 "phoenix down", "last man standing", "event horizon", "millionaires club",
									 "breakin the law", "gettin it",
									 "highscores"};
	private static string[] IDS = {"CgkIucKVsZ8TEAIQCA", "CgkIucKVsZ8TEAIQCQ", "CgkIucKVsZ8TEAIQCg", "CgkIucKVsZ8TEAIQCw",
								   "CgkIucKVsZ8TEAIQDA", "CgkIucKVsZ8TEAIQEg", "CgkIucKVsZ8TEAIQDg", "CgkIucKVsZ8TEAIQDw",
								   "CgkIucKVsZ8TEAIQEQ", "CgkIucKVsZ8TEAIQDQ",
								   "CgkIucKVsZ8TEAIQEA"};
	#endif
	
	private static SocialManager _instance;
	public static SocialManager Instance {
		get {
			if (!_instance) {
				_instance = GameObject.Find("SocialManager").GetComponent<SocialManager>();
			}
			return _instance;
		}
		private set {}
	}

	//Activate the play games platform
	void Awake(){

		//Update the button before self-destruction
		UpdateButton();

		//Check if already active (and destroy if so)
		if (SocialManager.Instance != this) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		busy = false;

		#if FALSE && UNITY_ANDROID
		GooglePlayGames.PlayGamesPlatform.Activate();
		//Set up human-readable mappings
		for (int i = 0 ; i < NAMES.Length; i++)
			((GooglePlayGames.PlayGamesPlatform) Social.Active).AddIdMapping(NAMES[i], IDS[i]);
		#elif UNITY_IPHONE
		GameCenterPlatform.Activate();
		#endif
	}

	void Start(){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (PlayerPrefs.GetInt("auto_login", 1) == 1){
			//Attempt to login to the service
			Login();
		}
		#endif
	}

	private void UpdateButton(){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		GameObject statusButton = GameObject.Find("Main Camera/BottomRight/socialStatus");
		if (statusButton){
			bool auth = Social.localUser.authenticated;
			statusButton.GetComponent<TextMesh>().text = auth ? "logged in" : "logged out";
			statusButton.gameObject.GetComponent<Animation>().Play("buttonpress");
		}
		#endif
	}
	
	public void Login () {
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (!Social.localUser.authenticated) {
			busy = true;
			try{
				Social.localUser.Authenticate((bool success) => {
					if (success){
						//Automatically log in in the future
						PlayerPrefs.SetInt("auto_login", 1);
					}
					else{
						//Login failed, don't try again unless the player asks
						PlayerPrefs.SetInt("auto_login", 0);
					}
					busy = false;
					UpdateButton();
				});
			}
			catch(Exception){
				//Exception is thrown when failing to call into the plugin code
				busy = false;
				UpdateButton();
			}
		}
		else{
			UpdateButton();
		}
		#endif
	}

	IEnumerator LogoutCo(){
		busy = true;
		try{
			#if FALSE
			((GooglePlayGames.PlayGamesPlatform)Social.Active).SignOut();
			while (Social.localUser.authenticated) {
				yield return new WaitForEndOfFrame();
			}
			#else
			yield return new WaitForEndOfFrame();
			#endif
			//Don't automatically log in in the future
			PlayerPrefs.SetInt("auto_login", 0);
		}
		finally{
			busy = false;
			UpdateButton();
		}
	}

	public void Logout(){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (Social.localUser.authenticated) {
			StartCoroutine(LogoutCo());
		}
		#endif
	}

	public void PostScore(string board, int score){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (Array.IndexOf (NAMES, board) < 0 || !Social.localUser.authenticated)
			return;

		busy = true;
		Social.ReportScore(score, board, (bool success) => {
			busy = false;
		});
		#endif
	}

	public void UnlockAchievement(string name){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (Array.IndexOf (NAMES, name) < 0 || !Social.localUser.authenticated)
			return;

		busy = true;
		Social.ReportProgress(name, 100.0f, (bool success) => {
			busy = false;
		});
		#endif
	}

	public void ShowAchievements(){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (!Social.localUser.authenticated)
			return;

		Social.ShowAchievementsUI();
		#endif
	}

	public void ShowLeaderboards(){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (!Social.localUser.authenticated)
			return;

		Social.ShowLeaderboardUI();
		#endif
	}

	public void ShowLeaderboards(string name){
		#if (UNITY_IPHONE || UNITY_ANDROID)
		if (!Social.localUser.authenticated)
			return;
		#endif

		#if FALSE && UNITY_ANDROID
		if (Array.IndexOf (NAMES, name) >= 0)
			((GooglePlayGames.PlayGamesPlatform)Social.Active).ShowLeaderboardUI(name);
		else
			Social.ShowLeaderboardUI();
		#elif (UNITY_IPHONE)
			Social.ShowLeaderboardUI();
		#endif
	}
}

using UnityEngine;
using System.Collections;

public class SignInButton : InteractiveObject {

	//Set up the social system
	public void Awake(){
		//TODO: This happens every time the menu is loaded

		#if UNITY_ANDROID
		Debug.Log ("Activating GPG/GC");
		GooglePlayGames.PlayGamesPlatform.Activate();
		
		//Set up human-readable mappings
		//Make sure these are the same as other social services
		((GooglePlayGames.PlayGamesPlatform) Social.Active).AddIdMapping("complete-takeover", "CgkIucKVsZ8TEAIQCQ");
		((GooglePlayGames.PlayGamesPlatform) Social.Active).AddIdMapping("hexciting", "CgkIucKVsZ8TEAIQCA");
		((GooglePlayGames.PlayGamesPlatform) Social.Active).AddIdMapping("30-move", "CgkIucKVsZ8TEAIQAg");
		((GooglePlayGames.PlayGamesPlatform) Social.Active).AddIdMapping("60-move", "CgkIucKVsZ8TEAIQBw");
		#elif UNITY_IPHONE
		//TODO: UNTESTED
		Debug.Log ("Activating GC");
		GameCenterPlatform.Activate();
		#else
		Debug.Log ("Not on mobile, not logging into GPG/GC");
		return;
		#endif
	}
	
	public override void TapAction () {
		//Attempt to login to the service
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
}

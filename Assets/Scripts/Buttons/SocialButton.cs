using UnityEngine;
using System.Collections;

public class SocialButton : InteractiveObject {

	public GameObject button = null;

	//Set up the social system
	public void Awake(){
		//TODO: This happens every time the menu is loaded
		
		#if UNITY_ANDROID
		button = transform.Find("gpgicon").gameObject;

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
		button = transform.Find("gcicon").gameObject;

		Debug.Log ("Activating GC");
		//GameCenterPlatform.Activate();
		#else
		Debug.Log ("Not on mobile, not logging into GPG/GC");
		return;
		#endif

		if (button != null) button.SetActive(true);
	}

	public void Start (){
		if (Social.localUser.authenticated){
			if (button != null) button.SetActive(false);
		}
	}

	public override void TapAction () {
		//Attempt to login to the service
		if (!Social.localUser.authenticated) {
			Social.localUser.Authenticate((bool success) => {
				if(success){
					if (button != null) button.SetActive(false);
				}
				else{
					Debug.Log("Couldn't log in");
				}
			});
		}
	}

	public void Shake(){
		gameObject.animation.Play();
	}
}

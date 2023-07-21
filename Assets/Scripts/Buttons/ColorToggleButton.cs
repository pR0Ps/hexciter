using UnityEngine;
using System.Collections;

public class ColorToggleButton : InteractiveObject {

	private int scheme;
	private HexaCube[] demos;
	private const int NUM_SCHEMES = 3;

	public void Awake(){
		scheme = PlayerPrefs.GetInt ("color_scheme", 0);
		demos = transform.parent.GetComponentsInChildren<HexaCube>();
	}

	public void Start(){
		for (int i = 0; i < demos.Length; i++) {
			demos[i].Spawn(Constants.ALL_COLORS[scheme][i]);
		}
	}

	private bool Busy(){
		for (int i = 0; i < demos.Length; i++) {
			if (demos[i].busy || !demos[i].alive)
				return true;
		}
		return false;
	}

	private void Respawn(){
		for (int i = 0; i < demos.Length; i++) {
			demos[i].Kill(Constants.ALL_COLORS[scheme][i]);
		}
	}
	
	public override void DownAction () {
		if (!Busy()){
			GetComponent<Animation>().Play("buttonpress");
			scheme = ++scheme % NUM_SCHEMES;
			PlayerPrefs.SetInt ("color_scheme", scheme);
			Respawn();
		}
	}
}
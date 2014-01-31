using UnityEngine;
using System.Collections;

public class ActionChooser : InteractiveObject {


	float time_opened;
	bool _visible;
	GridPlace current;

	void Awake(){
		current = null;
		_visible = false;
	}

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}

	//Hide the actionChooser
	void Hide(){
		if (_visible) {
			animation.Play ("hide");
			_visible = false;
		}
	}

	//Called when the user taps a grid place
	void GridSelect(GridPlace gp){
		if (current == gp) return;

		current = gp;

		if (_visible) {
			//Visible and another grid was tapped
			Hide ();
		}
		else{
			//Not visible, spawn the actionChooser
			time_opened = Time.time;
			_visible = true;
			transform.position = gp.transform.position - Vector3.forward * 5;
			animation.Play ("show");
		}
	}

	//Accessor for visiblity
	bool visible{
		get{
			return _visible;
		}
		set{}
	}
}

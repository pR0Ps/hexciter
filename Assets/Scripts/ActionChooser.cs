using UnityEngine;
using System.Collections;

public class ActionChooser : InteractiveObject {

	public static ActionChooser Instance {get; private set;}
	public bool Visible {get; private set;}

	void Awake () {
		Instance = this;
	}

	public void Show () {
		animation.Play ("show");
		Visible = true;
	}

	public void Hide () {
		animation.Play ("hide");
		Visible = false;
	}
}

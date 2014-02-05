using UnityEngine;
using System.Collections;

public class MenuButton : InteractiveObject {
	
	public override void TapAction () {
		Application.LoadLevel("menu");
	}
}

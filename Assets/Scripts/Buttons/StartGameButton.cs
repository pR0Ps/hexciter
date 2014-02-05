using UnityEngine;
using System.Collections;

public class StartGameButton : InteractiveObject {

	public override void TapAction () {
		Application.LoadLevel("game");
	}
}

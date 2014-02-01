using UnityEngine;
using System.Collections;

public class FloodButton : InteractiveObject {

	public override void DownAction () {
		GridLogic.Instance.Flood();
	}

	public override void UpAction () {
		GridLogic.Instance.Flood();
	}
}

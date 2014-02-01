using UnityEngine;
using System.Collections;

public class DestroyButton : InteractiveObject {

	public override void DownAction () {
		GridLogic.Instance.Destroy();
	}

	public override void UpAction () {
		GridLogic.Instance.Destroy();
	}
}

using UnityEngine;
using System.Collections;

public class ScreenShotTool : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.A)) {
			ScreenCapture.CaptureScreenshot("screenshot"+ System.DateTime.Now.ToString("yyyyMMddHHmmssfff") +".png", 2);
		}
	}
}

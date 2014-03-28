using UnityEngine;
using System.Collections;

public enum GUIElementPosition {TopLeft,TopCenter,TopRight,MiddleLeft,MiddleCenter,MiddleRight,BottomLeft,BottomCenter,BottomRight};

public class GUIAnchor : MonoBehaviour {
	
	public GUIElementPosition elementPosition;
	public Camera guiCamera;
	
	void Start () {
		RepositionSelf();
	}
	
	public void RepositionSelf () {
		if (!guiCamera)
			guiCamera = transform.parent.GetComponentInChildren<Camera>();
		
		if (elementPosition == GUIElementPosition.TopLeft) {
			transform.localPosition = new Vector3(-guiCamera.orthographicSize * guiCamera.aspect, guiCamera.orthographicSize, 0);
		}
		else if (elementPosition == GUIElementPosition.TopCenter) {
			transform.localPosition = new Vector3(0, guiCamera.orthographicSize, 0);
		}
		else if (elementPosition == GUIElementPosition.TopRight) {
			transform.localPosition = new Vector3(guiCamera.orthographicSize * guiCamera.aspect, guiCamera.orthographicSize, 0);
		}
		else if (elementPosition == GUIElementPosition.MiddleLeft) {
			transform.localPosition = new Vector3(-guiCamera.orthographicSize * guiCamera.aspect,0, 0);
		}
		else if (elementPosition == GUIElementPosition.MiddleCenter) {
			transform.localPosition = new Vector3(0, 0, 0);
		}
		else if (elementPosition == GUIElementPosition.MiddleRight) {
			transform.localPosition = new Vector3(guiCamera.orthographicSize * guiCamera.aspect, 0, 0);
		}
		else if (elementPosition == GUIElementPosition.BottomLeft) {
			transform.localPosition = new Vector3(-guiCamera.orthographicSize * guiCamera.aspect, -guiCamera.orthographicSize, 0);
		}
		else if (elementPosition == GUIElementPosition.BottomCenter) {
			transform.localPosition = new Vector3(0, -guiCamera.orthographicSize, 0);
		}
		else if (elementPosition == GUIElementPosition.BottomRight) {
			transform.localPosition = new Vector3(guiCamera.orthographicSize * guiCamera.aspect, -guiCamera.orthographicSize, 0);
		}
	}
}
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GUIAnchor))]
public class GUIAnchorEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();
		if (GUILayout.Button("Reposition")) {
			((GUIAnchor)target).RepositionSelf();
		}
	}
}

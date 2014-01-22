using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

public class DiscoverSiblings : Editor {

	//Add a menu item to the Unity Editor to call this function
	//Itterates over selected objects and discovers (and populates) siblings
	[MenuItem("Tools/Discover Siblings")]
	static void FindSibs () {
		foreach (GameObject go in Selection.gameObjects) {
			GridPlace gp = go.GetComponent<GridPlace>();
			if (gp)
				gp.DiscoverSiblings();
		}
	}

	//Attach a child with the polygon collider to each grid place
	[MenuItem("Tools/Attach Hexagon Collider")]
	static void AttachHexagonChild () {

		foreach (GameObject go in Selection.gameObjects) {
			GridPlace gp = go.GetComponent<GridPlace>();
			if (gp) {
				if (!gp.GetComponent<PolygonCollider2D>()) {
					if (gp.GetComponent<SphereCollider>())
						DestroyImmediate(gp.GetComponent<SphereCollider>());
					gp.gameObject.AddComponent<PolygonCollider2D>();
				}
				PolygonCollider2D hexCollider = (Resources.Load("PolygonCollider") as GameObject).GetComponent<PolygonCollider2D>();
				PolygonCollider2D newCol = gp.GetComponent<PolygonCollider2D>();
				newCol.SetPath(0, hexCollider.points);
			}
		}
	}
}

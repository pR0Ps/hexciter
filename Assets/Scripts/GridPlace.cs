using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Siblings {
	private List<GridPlace> existingSibs;
	
	public GridPlace NorthEast;
	public GridPlace East;
	public GridPlace SouthEast;
	public GridPlace SouthWest;
	public GridPlace West;
	public GridPlace NorthWest;
	
	public List<GridPlace> ExistingSibs () {
		if (existingSibs != null)
			return existingSibs;
		
		List<GridPlace> exSibs = new List<GridPlace>();
		
		if (NorthEast)
			exSibs.Add(NorthEast);
		if (East)
			exSibs.Add(East);
		if (SouthEast)
			exSibs.Add(SouthEast);
		if (SouthWest)
			exSibs.Add(SouthWest);
		if (West)
			exSibs.Add(West);
		if (NorthWest)
			exSibs.Add(NorthWest);
		
		existingSibs = exSibs;
		
		return existingSibs;
	}
}

public class GridPlace : InteractiveObject {

	private PlayerActions playerActions;

	public GameObject HexaCubePrefab;
	public HexaCube hexaCube;
	
	public bool busy {get; set;}

	Transform lookTransform;
	Quaternion targetRotation;

	Vector3 targetScale;
	Vector3 targetPosition;
	
	[SerializeField]
	public Siblings sibs;
	
	public void Awake () {
		playerActions = GameObject.Find("PlayerActions").GetComponent<PlayerActions>();

		hexaCube = (Object.Instantiate(HexaCubePrefab) as GameObject).GetComponent<HexaCube>();
		hexaCube.transform.parent = transform;
		hexaCube.transform.localPosition = Vector3.zero;
		hexaCube.gridPlace = this;
		lookTransform = hexaCube.transform.FindChild ("LookRotation");

		targetScale = Vector3.one;
		targetPosition = transform.localPosition;

		busy = true;
	}

	public override void DownAction () {
		playerActions.DownAction(this);
	}

	public void Scale(int depth, bool scale){
		if (scale) {
			//Scale
			if (depth == 0){
				//I was the chosen one
				targetScale = Vector3.one * 1.2f;
				targetPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2);
			}
			else{
				targetScale = Vector3.one * (float)(0.3 + 0.7 * (depth/3f));
			}
		}
		else{
			//Return to normal
			targetScale = Vector3.one;
			targetPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
		}
	}

	void Update () {
		if (!busy){
			if (playerActions.swiping) {
				Vector3 lookPoint = new Vector3 (InputHandler.Instance.inputVectorWorld.x, InputHandler.Instance.inputVectorWorld.y, Constants.CUBE_LOOK_DIST);
				targetRotation = Quaternion.LookRotation(lookPoint - lookTransform.position);
			}
			else
				targetRotation = Quaternion.identity;

			lookTransform.rotation = Quaternion.Slerp (lookTransform.rotation, targetRotation, Constants.CUBE_LOOK_SPEED * Time.deltaTime);
		}

		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 10f * Time.deltaTime);
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 10f * Time.deltaTime);
	}
	
	//An editor utility function to populate sibling lists
	//Called by in Editor/DiscoverSiblings.cs
	//Casts 6 rays out in a hexagon to discover siblings
	public void DiscoverSiblings () {
		
		Collider2D hit;
		
		//NorthEas Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(0.8f, 1.38f))) {
			sibs.NorthEast = hit.transform.GetComponent<GridPlace>();
		}
		//East Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(1.6f, 0f))) {
			sibs.East = hit.transform.GetComponent<GridPlace>();
		}
		//SouthEast Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(0.8f, -1.38f))) {
			sibs.SouthEast = hit.transform.GetComponent<GridPlace>();
		}
		//SouthWest Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(-0.8f, -1.38f))) {
			sibs.SouthWest = hit.transform.GetComponent<GridPlace>();
		}
		//West Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(-1.6f, 0f))) {
			sibs.West = hit.transform.GetComponent<GridPlace>();
		}
		//NorthWest Raycast
		if (hit = Physics2D.OverlapPoint((Vector2)transform.position + new Vector2(-0.8f, 1.38f))) {
			sibs.NorthWest = hit.transform.GetComponent<GridPlace>();
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.white;
		if (sibs.NorthEast)
			Gizmos.DrawLine(transform.position, sibs.NorthEast.transform.position);
		if (sibs.East)
			Gizmos.DrawLine(transform.position, sibs.East.transform.position);
		if (sibs.SouthEast)
			Gizmos.DrawLine(transform.position, sibs.SouthEast.transform.position);
		if (sibs.SouthWest)
			Gizmos.DrawLine(transform.position, sibs.SouthWest.transform.position);
		if (sibs.West)
			Gizmos.DrawLine(transform.position, sibs.West.transform.position);
		if (sibs.NorthWest)
			Gizmos.DrawLine(transform.position, sibs.NorthWest.transform.position);
	}
}

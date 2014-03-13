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
	public GameObject HexaCubePrefab;
	public HexaCube hexaCube;
	
	public bool busy;
	public bool alive;
	
	public bool scored;
	
	[SerializeField]
	public Siblings sibs;
	
	public void Awake () {
		hexaCube = (Object.Instantiate(HexaCubePrefab) as GameObject).GetComponent<HexaCube>();
		hexaCube.transform.parent = transform;
		hexaCube.transform.localPosition = Vector3.zero;
		hexaCube.gridPlace = this;
		hexaCube.EnableLookRotation ();
	}
	
	public override void DownAction () {
		PlayerActions.Instance.DownAction(this);
	}
	
	public int TallyScore (Constants.HexColors rootColor) {
		scored = true;
		int tally = 1;
		List<GridPlace> existingSibs = sibs.ExistingSibs();
		for (int i = 0; i < existingSibs.Count; i ++) {
			if (!existingSibs[i].scored && existingSibs[i].hexaCube.hexColor == rootColor) {
				tally += existingSibs[i].TallyScore(rootColor);
			}
		}
		return tally;
	}
	
	public bool Fill (Constants.HexColors fillColor) {
		if (!busy && alive) {
			busy = true;
			StartCoroutine(FillSiblings(hexaCube.hexColor, fillColor));
			hexaCube.Fill(fillColor);
			return true;
		}
		return false;
	}
	
	IEnumerator FillSiblings (Constants.HexColors rootColor, Constants.HexColors fillColor) {
		yield return new WaitForSeconds (0.1f);
		List<GridPlace> existingSibs = sibs.ExistingSibs();
		for (int i = 0; i < existingSibs.Count; i ++) {
			if (existingSibs[i].hexaCube.hexColor == rootColor) {
				existingSibs[i].Fill(fillColor);
			}
		}
	}
	
	public void Kill () {
		if (!busy && alive) {
			scored = false;
			busy = true;
			StartCoroutine(KillSiblings(hexaCube.hexColor));
			hexaCube.Kill();
		}
	}
	
	IEnumerator KillSiblings (Constants.HexColors rootColor) {
		yield return new WaitForSeconds (0.1f);
		List<GridPlace> existingSibs = sibs.ExistingSibs();
		for (int i = 0; i < existingSibs.Count; i ++) {
			if (existingSibs[i].hexaCube.hexColor == rootColor) {
				existingSibs[i].Kill();
			}
		}
	}

	public void SlowSpawn () {
		if (!busy && !alive) {
			busy = true;
			hexaCube.SlowSpawn();
			StartCoroutine(SlowSpawnSiblings());
		}
	}
	
	private IEnumerator SlowSpawnSiblings () {
		yield return new WaitForSeconds(.3f);
		foreach (GridPlace gp in sibs.ExistingSibs()) {
			gp.SlowSpawn();
		}
	}
	
	public void Despawn () {
		if (!busy && alive) {
			busy = true;
			hexaCube.Despawn();
			StartCoroutine(DespawnSiblings());
		}
	}
	
	private IEnumerator DespawnSiblings () {
		yield return new WaitForSeconds(.1f);
		foreach (GridPlace gp in sibs.ExistingSibs()) {
			gp.Despawn();
		}
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

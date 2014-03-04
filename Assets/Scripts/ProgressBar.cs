using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	public static ProgressBar Instance;
	HexaCube[] progressCubes = new HexaCube[9]; // populated with the 10 child hexacubes

	int moveNumber; // starts at 0, count up towards 9
	
	void Awake () {
		Instance = this;
		
		// Setup references to child hexacubes (in the correct order)
		for (int i=0; i<=8; i++)
			progressCubes[i] = transform.FindChild ((i+1).ToString()).GetComponent<HexaCube> ();
	}
	
	void Start () {
		for (int i=0; i < progressCubes.Length; i++) {
			progressCubes[i].SlowSpawn(Constants.HexColors.Grey);
		}
	}
}
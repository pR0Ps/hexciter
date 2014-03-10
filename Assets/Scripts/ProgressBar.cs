using UnityEngine;
using System.Collections;

public class ProgressBar : MonoBehaviour {
	public static ProgressBar Instance;
	public const int NUM_HEXES = 9;
	HexaCube[] progressCubes = new HexaCube[NUM_HEXES]; // populated with the 9 child hexacubes
	bool[] state = new bool[NUM_HEXES];

	void Awake () {
		Instance = this;
		
		// Setup references to child hexacubes (in the correct order)
		for (int i = 0; i < NUM_HEXES; i++)
			progressCubes[i] = transform.FindChild ((i).ToString()).GetComponent<HexaCube>();
	}
	
	void Start () {
		for (int i = 0; i < NUM_HEXES; i++) {
			state[i] = false;
			SpawnCube(i);
		}
	}

	void SpawnCube(int i){
		if (!state[i]){
			progressCubes[i].SlowSpawn(Constants.HexColors.Grey);
			state[i] = true;
		}
	}

	void DestroyCube(int i){
		if (state[i]){
			progressCubes[i].Despawn();
			state[i] = false;
		}
	}

	public void SetPercent(float percent){
		percent = Mathf.Min (Mathf.Max(percent, 0), 1);

		for (int i = 0; i < Mathf.FloorToInt(NUM_HEXES * percent); i++) {
			DestroyCube(i);
		}
		for (int i = Mathf.FloorToInt(NUM_HEXES * percent); i < NUM_HEXES; i++) {
			SpawnCube(i);
		}
	}
}
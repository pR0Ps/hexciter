using UnityEngine;
using System.Collections;

public class MoveProgress : MonoBehaviour {

	public const int MAX_MOVES = 9;
	HexaCube[] progressCubes = new HexaCube[MAX_MOVES]; // populated with the 9 child hexacubes
	bool[] state = new bool[MAX_MOVES];

	int _moves;
	void Awake () {

		_moves = 0;
		
		// Setup references to child hexacubes (in the correct order)
		for (int i = 0; i < MAX_MOVES; i++)
			progressCubes[i] = transform.FindChild ((i).ToString()).GetComponent<HexaCube>();
	}
	
	void Start () {
		for (int i = 0; i < MAX_MOVES; i++) {
			state[i] = false;
			SpawnCube(i);
		}
	}

	void SpawnCube(int i){
		if (!state[i]){
			progressCubes[i].SlowSpawn(Constants.GREY);
			state[i] = true;
		}
	}

	void DestroyCube(int i){
		if (state[i]){
			progressCubes[i].Despawn();
			state[i] = false;
		}
	}

	private int moves {
		get {
			return _moves;
		}
		set {
			_moves = value;
			SetPercent(_moves/(float)MAX_MOVES);
		}
	}

	public bool NoneLeft(){
		return moves >= MAX_MOVES;
	}

	public void DoMove(){
		moves++;
	}

	public int Remaining(){
		return MAX_MOVES - moves;
	}

	public void ResetMoves(int scorePerCube){
		for (int i = 0; i < MAX_MOVES; i++) {
			if (state[i]) ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (scorePerCube, progressCubes[i].transform.position);
		}
		moves = 0;
	}

	public void SetPercent(float percent){
		percent = Mathf.Min (Mathf.Max(percent, 0), 1);

		for (int i = 0; i < Mathf.FloorToInt(MAX_MOVES * percent); i++) {
			DestroyCube(i);
		}
		for (int i = Mathf.FloorToInt(MAX_MOVES * percent); i < MAX_MOVES; i++) {
			SpawnCube(i);
		}
	}
}
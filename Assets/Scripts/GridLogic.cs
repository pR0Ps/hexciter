﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {

	public static GridLogic Instance;

	GridPlace origin;
	GridPlace northWestCorner;
	bool spawned;

	bool gameover;

	public AnimationCurve ScoreCurve;
	int score;
	int level;
	int moves;
	int maxmoves;
	int minscore;
	public TextMesh scoreTextMesh;
	public TextMesh gameOverTextMesh;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
	}

	void Start () {
		gameover = false;
		level = 1;
		ColorSelector.Instance.Init ();
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	void NextLevel(){
		level++;
		maxmoves = ProgressBar.NUM_HEXES;
		moves = 0;
		minscore = score + level * 2000;
		ProgressBar.Instance.SetPercent(moves/(float)maxmoves);
	}

	public void Flood(GridPlace start) {
		if (start.Fill (ColorSelector.Instance.Current())) {
			DoMove();
		}
	}

	public void Destroy (GridPlace start) {
		int multiplier = 1;
		if (ColorSelector.Instance.Current () == start.hexaCube.hexColor) multiplier = 2;

		int tally = start.TallyScore(start.hexaCube.hexColor);
		int earnedScore = tally * multiplier * 100;

		score += earnedScore;
		scoreTextMesh.text = score.ToString ("N0");
		start.Kill ();

		ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (earnedScore, start.transform.position);
		DoMove ();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves++;
		ColorSelector.Instance.NewColor();
		ProgressBar.Instance.SetPercent(moves/(float)maxmoves);
	}
	
	void Update () {

		if (moves >= maxmoves && score < minscore) {
			if (!gameover){
				gameover = true;
				gameOverTextMesh.gameObject.SetActive(true);

				#if (UNITY_IPHONE || UNITY_ANDROID)
				Social.ReportScore(score, "30-move", (bool success) => {
					if(success){
						Debug.Log("Posted score!");
					}
					else{
						Debug.Log("Couldn't post score");
					}
				});
				#endif
			}

			if (InputHandler.Instance.inputSignalDown)
				Application.LoadLevel("menu");

			return;
		}
		else if (moves >= maxmoves){
			NextLevel();
		}
	}
}

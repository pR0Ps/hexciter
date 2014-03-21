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
	private MoveProgress moves;
	int score;
	int lastMoveScore;
	int level;
	int minscore;
	public TextMesh scoreTextMesh;
	public TextMesh targetTextMesh;
	public TextMesh gameOverTextMesh;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
		moves = MoveProgress.Instance;
	}

	void Start () {
		gameover = false;
		level = 1;
		score = 0;
		minscore = 4000;
		targetTextMesh.text = minscore.ToString("N0");
		ColorSelector.Instance.Init ();
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		StartCoroutine(Utils.SlowSpawnSiblings(origin));
	}

	void UpdateUI(){
		targetTextMesh.text = minscore.ToString("N0");
		scoreTextMesh.text = score.ToString("N0");
	}

	void NextLevel(){
		int scorePer = level * 500;
		score += moves.Remaining() * scorePer;
		moves.ResetMoves(scorePer);

		level++;
		minscore = score + level * 2000;
		UpdateUI();
	}

	public void Flood(GridPlace start) {
		lastMoveScore /= 2; // halves your combo bonus
		if (!start.busy && start.alive){
			Utils.ReserveAll(start);
			StartCoroutine(Utils.FillSiblings(start, ColorSelector.Instance.Current()));
			DoMove();
		}
	}

	public void Destroy (GridPlace start) {
		int multiplier = 1;
		if (ColorSelector.Instance.Current () == start.hexaCube.hexColor) multiplier = 2;

		int tally = Utils.TallyScore(start);
		int earnedScore = ((int)(lastMoveScore * (tally/61f) + tally * 100)/10) * 10 * multiplier;
		lastMoveScore = earnedScore;

		score += earnedScore;
		UpdateUI();
		StartCoroutine(Utils.KillSiblings(start));

		ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (earnedScore, start.transform.position);
		DoMove ();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves.DoMove();
		ColorSelector.Instance.NewColor();
	}
	
	void Update () {

		if (score >= minscore){
			NextLevel();
		}
		else if (moves.NoneLeft()){
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
		}
	}
}

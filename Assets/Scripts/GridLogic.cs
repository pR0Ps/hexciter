using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {

	private ColorSelector colorSelector;

	GridPlace origin;
	GridPlace northWestCorner;
	bool spawned;

	bool gameover;
	public bool disabled {get; set;}

	public AnimationCurve ScoreCurve;
	private MoveProgress moves;
	int score;
	int lastMoveScore;
	int level;
	int startScore;
	int targetScore;
	ScoreBar scoreBar;
	public TextMesh scoreTextMesh;
	public TextMesh gameOverTextMesh;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		scoreBar = GameObject.Find ("ScoreBar").GetComponent<ScoreBar> ();
		moves = GameObject.Find("GUICamera/MoveProgress").GetComponent<MoveProgress>();
		colorSelector = GameObject.Find("GUICamera/ColorSelector").GetComponent<ColorSelector>();
		disabled = false;
	}

	void Start () {
		gameover = false;
		level = 1;
		score = 0;
		startScore = 0;
		targetScore = 4000;
		colorSelector.Init ();
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		StartCoroutine(Utils.SlowSpawnSiblings(origin));
	}

	void UpdateUI(){
		scoreTextMesh.text = score.ToString("N0");
		scoreBar.ReportProgress (score, startScore, targetScore);
	}

	IEnumerator NextLevel(){
		disabled = true;
		GridPlace[] all = Utils.Unpack<GridPlace>(Utils.GetSiblings(origin));

		while (!all.All(gp => gp.alive && !gp.busy)){
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(0.2f);

		int scorePer = level * 500;
		score += moves.Remaining() * scorePer;
		moves.ResetMoves(scorePer);

		level++;
		startScore = score;
		targetScore = score + level * 2000;
		UpdateUI();

		disabled = false;
	}

	IEnumerator SpawnBlack(GridPlace[] gps){
		while (disabled) {
			yield return new WaitForEndOfFrame();
		}
		//create black hexes
		foreach (GridPlace gp in gps){
			gp.MakeBlack();
		}

		yield return new WaitForSeconds(2f);

		if (gameover) {
			EndGame();
		}
	}

	void EndGame(){
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

	public void Flood(GridPlace start) {
		lastMoveScore /= 2; // halves your combo bonus
		if (!start.busy && start.alive){
			Utils.ReserveAll(start);
			StartCoroutine(Utils.FillSiblings(start, colorSelector.Current()));
			DoMove();
		}
	}

	public void Destroy (GridPlace start) {
		int multiplier = 1;
		if (colorSelector.Current () == start.hexaCube.hexColor) multiplier = 2;

		int tally = Utils.TallyScore(start);
		int earnedScore = ((int)(lastMoveScore * (tally/61f) + tally * 100)/10) * 10 * multiplier;
		lastMoveScore = earnedScore;

		score += earnedScore;

		UpdateUI();
		StartCoroutine(Utils.KillSiblings(start));

		ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (earnedScore, start.transform.position);
		DoMove ();
	}

	//Anytime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves.DoMove();
		colorSelector.NewColor();
	}
	
	void Update () {

		if (disabled) return;

		if (score >= targetScore){
			StartCoroutine(NextLevel());
		}
		else if (moves.NoneLeft()){
			int remain = scoreBar.HexesRemaining();
			GridPlace[] not_black = Utils.GetRandomNonBlack(origin, remain);

			if(not_black.Length < remain){
				if (!gameover){
					gameover = true;
					StartCoroutine(SpawnBlack(not_black));
				}
				else if (InputHandler.Instance.inputSignalDown && gameOverTextMesh.gameObject.activeSelf){
					Application.LoadLevel("menu");
				}
			}
			else{
				StartCoroutine(NextLevel());
				StartCoroutine(SpawnBlack(not_black));
			}
		}
	}
}

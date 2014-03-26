using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {

	private ColorSelector colorSelector;

	GridPlace origin;
	GridPlace[] gridPlaces;

	bool gameover;

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
	public bool disabled { get; private set; }

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		scoreBar = GameObject.Find ("ScoreBar").GetComponent<ScoreBar> ();
		moves = GameObject.Find("GUICamera/MoveProgress").GetComponent<MoveProgress>();
		colorSelector = GameObject.Find("GUICamera/ColorSelector").GetComponent<ColorSelector>();
	}

	void Start () {
		gameover = false;
		level = 1;
		score = 0;
		startScore = 0;
		targetScore = 4000;
		colorSelector.Init ();
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		gridPlaces = GetComponentsInChildren<GridPlace> ();

		StartCoroutine(Utils.SlowSpawnSiblings(origin));
	}

	void UpdateUI(){
		scoreBar.ReportProgress (score, startScore, targetScore);
		scoreTextMesh.text = score.ToString("N0");
	}

	void NextLevel(){

		//Calculate bonus points
		int scorePer = level * 500;
		score += moves.Remaining() * scorePer;

		//Calculate how many black hexes to add
		int leftover = scoreBar.NumBlackHexes ();

		//Setup next level
		moves.ResetMoves(scorePer);
		level++;
		startScore = score;
		targetScore = score + level * 2000;
		
		UpdateUI();

		//coroutine waits for everything to be at rest
		StartCoroutine (LevelCleanup (leftover));

	}

	IEnumerator LevelCleanup (int leftover) {

		disabled = true;

		while (true) {
			int i = 0;
			for (i = 0; i < gridPlaces.Length; i++) {
				if (gridPlaces[i].busy || gridPlaces[i].reserved) break;
			}
			if (i >= gridPlaces.Length) {
				break;
			}
			yield return new WaitForEndOfFrame();
		}

		if (leftover != 0) {
			GridPlace[] newBlacks = Utils.GetRandomNonBlack(origin, leftover);
			for (int i = 0; i < newBlacks.Length; i++) {
				newBlacks[i].hexaCube.spawnBlack = true;
				newBlacks[i].hexaCube.Kill();
			}
		}

		yield return new WaitForSeconds (1f);

		disabled = false;
	}


	public void Flood(GridPlace start) {
		lastMoveScore /= 2; // halves your combo bonus
		if (!start.busy){
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

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves.DoMove();
		colorSelector.NewColor();
	}

	void GameOverLogic () {
		if (!gameover) {
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

	void Update () {

		if (score >= targetScore || moves.NoneLeft()){
			NextLevel();
		}

		if (gameover) {
			// game over stuff
		}
	}
}

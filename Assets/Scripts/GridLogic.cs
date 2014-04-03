using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using sys = System; //System.Random conflicts with UnityEngine.Random

public class GridLogic : MonoBehaviour {

	private ColorSelector colorSelector;

	GridPlace origin;
	GridPlace[] gridPlaces = new GridPlace[61];
	int deadPointer = 0; //everything less than this is dead
	public AnimationCurve ScoreCurve;
	private MoveProgress moves;
	int score;
	int lastMoveScore;
	int level;
	int startScore;
	int targetScore;
	ScoreBar scoreBar;
	public TextMesh scoreTextMesh;
	public GameoverDialog gameoverDialog;
	public bool disabled { get; private set; }

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		scoreBar = GameObject.Find ("GUICamera").GetComponentInChildren<ScoreBar> ();
		moves = GameObject.Find("GUICamera").GetComponentInChildren<MoveProgress>();
		colorSelector = GameObject.Find("GUICamera").GetComponentInChildren<ColorSelector>();
	}

	void Start () {
		level = 1;
		score = 0;
		startScore = 0;
		targetScore = 5000;
		colorSelector.Init ();
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		
		for (int i = 1; i < 61; i++) {
			gridPlaces[i-1] = transform.FindChild(i.ToString()).GetComponent<GridPlace>();
		}
		gridPlaces[60] = origin;

		FadeCam.Instance.FadeIn(() => {StartCoroutine(StartGame());});
	}

	IEnumerator StartGame(){
		disabled = true;
		StartCoroutine(Utils.SlowSpawnSiblings(origin));
		while (GridBusy()) {
			yield return new WaitForEndOfFrame();
		}
		disabled = false;
	}

	void UpdateUI(bool lose){
		scoreBar.ReportProgress (score, startScore, targetScore, lose);
		scoreTextMesh.text = score.ToString("N0");
	}

	bool GridBusy () {
		for (int i = 0; i < gridPlaces.Length; i++) {
			if (gridPlaces[i].busy) {
				return true;
			}
		}
		return false;
	}

	IEnumerator NextLevel () {
		disabled = true;

		while (GridBusy()) {
			yield return new WaitForEndOfFrame();
		}

		//Calculate bonus points
		int scorePer = level * 200;
		score += moves.Remaining() * scorePer;

		//How many (if any) white hexes should be revived
		int revived = moves.Remaining ();

		//Calculate how many white hexes to add
		int leftover = scoreBar.NumBlackHexes ();

		//Setup next level
		moves.ResetMoves(scorePer);
		level++;
		startScore = score;
		targetScore = score + level * 5000;

		UpdateUI(leftover != 0);

		bool gameover = false;

		if (revived != 0) {
			int target = deadPointer - revived;
			for (; deadPointer > target && deadPointer > 0; deadPointer--) {
				GridPlace newRevived = gridPlaces[deadPointer - 1];
				newRevived.hexaCube.Spawn();
				yield return new WaitForSeconds (0.07f);
			}
		}

		else if (leftover != 0) {
			int target = deadPointer + leftover;
			for (; deadPointer < target && deadPointer < gridPlaces.Length; deadPointer++) {
				GridPlace newWhite = gridPlaces[deadPointer];
				newWhite.hexaCube.Despawn();
				yield return new WaitForSeconds (0.07f);
			}
		}

		if (deadPointer > 60) {
			gameover = true;
		}
		
		yield return new WaitForSeconds (.7f);

		disabled = false;

		if (gameover) {
			GameOver();
		}
	}

	public void Flood(GridPlace start) {
		lastMoveScore /= 4; // halves your combo bonus
		if (!start.busy){
			StartCoroutine(Utils.FillSiblings(start, colorSelector.Current()));
			AudioController.Instance.PlaySound ("fillchime");
			DoMove();
		}
	}

	//Return a callback for the current kill action
	public sys.Action<GridPlace, int> KillCallback(GridPlace start, Color color){
		Color startColor = start.hexaCube.hexColor;
		return (gp, count) => {
			if (gp == null){
				//Done counting, award score
				int levelBonus = 100 + (level - 1) * 10;
				int multiplier = startColor == color ? 2 : 1;
				int bonus = (int)(lastMoveScore * Mathf.Sqrt(count/61f));
				int earnedScore = ((bonus + count * levelBonus)/10) * 10 * multiplier;

				lastMoveScore = earnedScore;

				score += earnedScore;
				UpdateUI(false);

				ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show(earnedScore, start.transform.position);
			}
			else{
				if (count == 10) {
					AudioController.Instance.PlaySound ("chime2");
				}
				else if (count == 20) {
					AudioController.Instance.PlaySound ("chime3");
				}
				//Still counting, do animations and stuff
				//ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show(count, gp.transform.position);
			}
		};
	}

	public void Destroy (GridPlace start) {
		StartCoroutine(Utils.KillSiblings(start, KillCallback(start, colorSelector.Current())));
		AudioController.Instance.PlaySound ("chime1");
		DoMove();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves.DoMove();
		colorSelector.NewColor();
	}

	void GameOver () {

		int best = PlayerPrefs.GetInt ("Best_Score");

		if (score > best) {
			PlayerPrefs.SetInt ("Best_Score", score);
			best = score;
		}

		gameoverDialog.Show (score, best);

		SocialManager.Instance.PostScore("highscores", score);
	}

	void Update () {
		if (!disabled && (score >= targetScore || moves.NoneLeft())){
			StartCoroutine (NextLevel());
		}
	}
}

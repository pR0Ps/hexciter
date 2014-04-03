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

	bool tutorialEnabled = false;
	bool flooded = false;
	bool destroyed = false;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		scoreBar = GameObject.Find ("GUICamera").GetComponentInChildren<ScoreBar> ();
		moves = GameObject.Find("GUICamera").GetComponentInChildren<MoveProgress>();
		colorSelector = GameObject.Find("GUICamera").GetComponentInChildren<ColorSelector>();
	}

	void Start () {
		disabled = true;
		if (Application.loadedLevelName == "tutorial")
			tutorialEnabled = true;
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		
		for (int i = 1; i < 61; i++) {
			gridPlaces[i-1] = transform.FindChild(i.ToString()).GetComponent<GridPlace>();
		}
		gridPlaces[60] = origin;

		if (!tutorialEnabled)
			FadeCam.Instance.FadeIn(() => {StartCoroutine(StartGame());});
		else
			FadeCam.Instance.FadeIn(() => {StartCoroutine(Tutorial());});
	}

	IEnumerator StartGame() {
		level = 1;
		score = 0;
		startScore = 0;
		targetScore = 5000;
		colorSelector.Init ();
		
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
			flooded = true;
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

				//Clear all hexes in a single move
				if (count >= gridPlaces.Length){
					SocialManager.Instance.UnlockAchievement("total annihilation");
				}
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
		destroyed = true;
		StartCoroutine(Utils.KillSiblings(start, KillCallback(start, colorSelector.Current())));
		AudioController.Instance.PlaySound ("chime1");
		DoMove();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves.DoMove();
		if (!tutorialEnabled)
			colorSelector.NewColor();
		else
			colorSelector.NewColor(0);
	}
	
	void GameOver () {

		int best = PlayerPrefs.GetInt("Best_Score", 0);
		int plays = PlayerPrefs.GetInt("num_plays", 0) + 1;
		PlayerPrefs.SetInt ("num_plays", plays);

		if (level <= 3) {
			SocialManager.Instance.UnlockAchievement("everyone wins");
		}

		if (plays >= 10) {
			SocialManager.Instance.UnlockAchievement("hexciting");
		}

		if (score > best) {
			PlayerPrefs.SetInt ("Best_Score", score);
			best = score;
		}

		gameoverDialog.Show (score, best);

		SocialManager.Instance.PostScore("highscores", score);
	}

	void Update () {
		if (!disabled && !tutorialEnabled &&(score >= targetScore || moves.NoneLeft())){
			StartCoroutine (NextLevel());
		}
	}

	IEnumerator Tutorial() {
		
		TutorialText tText = GameObject.Find ("TutorialText").GetComponentInChildren<TutorialText> ();

		gridPlaces[60].hexaCube.SlowSpawn(Constants.ChooseColor(2)); 
		gridPlaces[59].hexaCube.SlowSpawn(Constants.ChooseColor(2)); 
		gridPlaces[58].hexaCube.SlowSpawn(Constants.ChooseColor(0)); 
		gridPlaces[57].hexaCube.SlowSpawn(Constants.ChooseColor(2)); 
		gridPlaces[56].hexaCube.SlowSpawn(Constants.ChooseColor(0)); 
		gridPlaces[55].hexaCube.SlowSpawn(Constants.ChooseColor(0)); 
		gridPlaces[54].hexaCube.SlowSpawn(Constants.ChooseColor(0)); 

		colorSelector.Init (0, 3);
		colorSelector.DisableSwap ();

		startScore = 0;
		targetScore = 4000;

		bool passed = false;

		yield return StartCoroutine(ShowWaitTap (tText, "welcome to hexciter", "(tap to continue)"));

		while (!passed) {
			yield return StartCoroutine(tText.Show ("press and hold a hex to select", "select the center hex"));
			yield return StartCoroutine(WaitForCenterSelect ());
			yield return StartCoroutine(tText.Hide ());
			colorSelector.GetComponentInChildren<TutorialArrow> ().FadeIn ();
			yield return StartCoroutine(tText.Show ("swipe left to flood", "this uses the color shown here"));
			yield return StartCoroutine(WaitForMoveCenter ());
			colorSelector.GetComponentInChildren<TutorialArrow> ().FadeOut ();
			disabled = true;

			if (!flooded && !destroyed) {
				yield return StartCoroutine(tText.Hide ());
				continue;
			}

			if (flooded) {
				passed = true;
				break;
			}

			if (destroyed) {
				break;
			}
		}
		if (!passed) {
			StartCoroutine(FailInstructions());
			yield break;
		}
		yield return StartCoroutine(tText.Hide ());
		yield return StartCoroutine(ShowWaitTap (tText, "well done!", "the hexes changed color"));
		colorSelector.GetComponentInChildren<TutorialArrow> ().FadeIn ();
		yield return StartCoroutine(ShowWaitTap (tText, "you'll always flood using this color", "(tap to continue)"));
		yield return StartCoroutine(tText.Show ("tap the palette to swap colors", ""));
		colorSelector.EnableSwap ();
		yield return StartCoroutine (WaitForSwap ());
		colorSelector.DisableSwap ();
		yield return StartCoroutine(tText.Hide ());
		colorSelector.GetComponentInChildren<TutorialArrow> ().FadeOut ();

		passed = false;
		flooded = false;
		destroyed = false;

		while (!passed) {
			yield return StartCoroutine(tText.Show ("select the center hex", ""));
			yield return StartCoroutine(WaitForCenterSelect ());
			yield return StartCoroutine(tText.Hide ());
			yield return StartCoroutine(tText.Show ("swipe right to destroy!", ""));
			yield return StartCoroutine(WaitForMoveCenter ());
			disabled = true;
			
			if (!flooded && !destroyed) {
				yield return StartCoroutine(tText.Hide ());
				continue;
			}
			
			if (destroyed) {
				passed = true;
				break;
			}
			
			if (flooded) {
				break;
			}
		}
		if (!passed) {
			StartCoroutine(FailInstructions());
			yield break;
		}

		yield return StartCoroutine(tText.Hide ());
		yield return StartCoroutine(ShowWaitTap (tText, "destroying hexes earns points", "match the color palette to earn bonus points!"));
		GameObject.Find ("GUICamera/MiddleRight").GetComponentInChildren<TutorialArrow> ().FadeIn ();
		yield return StartCoroutine(ShowWaitTap (tText, "as you earn points, this bar fills", "(tap to continue)"));
		GameObject.Find ("GUICamera/MiddleRight").GetComponentInChildren<TutorialArrow> ().FadeOut ();
		GameObject.Find ("GUICamera/MoveProgress/tutorialArrow1").GetComponent<TutorialArrow> ().FadeIn ();
		yield return StartCoroutine(ShowWaitTap (tText, "each turn uses a grey hex", "(tap to continue)"));
		yield return StartCoroutine(ShowWaitTap (tText, "fill the bar before you use them all!", "(tap to continue)"));
		GameObject.Find ("GUICamera/MoveProgress/tutorialArrow1").GetComponent<TutorialArrow> ().FadeOut ();

		for (int i = 0; i < 54; i++) {
			gridPlaces[i].hexaCube.SlowSpawn(Constants.RandomColor()); 
		}

		for (int i = 0; i < 6; i++) {
			moves.DoMove();
		}

		//GameObject.Find ("GUICamera/MoveProgress/tutorialArrow2").GetComponent<TutorialArrow> ().FadeIn ();

		yield return StartCoroutine(tText.Show ("try making a move yourself", "you have one turn left"));
		disabled = false;
		flooded = false;
		destroyed = false;
		colorSelector.EnableSwap ();
		targetScore = 8000;
		yield return StartCoroutine (WaitForMove ());
		//GameObject.Find ("GUICamera/MoveProgress/tutorialArrow2").GetComponent<TutorialArrow> ().FadeOut ();
		while (GridBusy())
			yield return new WaitForSeconds(0.1f);
		StartCoroutine (NextLevel ());
		while (GridBusy())
			yield return new WaitForSeconds(0.1f);
		yield return StartCoroutine(tText.Hide ());
		disabled = true;
		colorSelector.DisableSwap ();

		yield return StartCoroutine(ShowWaitTap (tText, "you ran out of turns", "before the score bar was filled"));
		yield return StartCoroutine(ShowWaitTap (tText, "when the board is empty...", "it's gameover!"));
		yield return StartCoroutine(ShowWaitTap (tText, "now you're ready to play!", "tap to start the game"));
		SocialManager.Instance.UnlockAchievement ("hello world");
		FadeCam.Instance.FadeOut(() => {Application.LoadLevel("game");});
	}

	IEnumerator FailInstructions (){
		TutorialText tText = GameObject.Find ("TutorialText").GetComponentInChildren<TutorialText> ();
		yield return StartCoroutine(tText.Hide ());

		SocialManager.Instance.UnlockAchievement("breakin the law");

		yield return StartCoroutine(ShowWaitTap (tText, "not going to follow intructions?", ""));
		yield return StartCoroutine(ShowWaitTap (tText, "maybe you don't need a tutorial...", ""));
		yield return StartCoroutine(ShowWaitTap (tText, "...so good luck!", "tap to start the game"));
		FadeCam.Instance.FadeOut(() => {Application.LoadLevel("game");});
	}

	IEnumerator ShowWaitTap (TutorialText tText, string s1, string s2) {
		yield return StartCoroutine(tText.Show (s1, s2));
		yield return StartCoroutine (WaitForTap ());
		yield return StartCoroutine(tText.Hide ());
	}

	IEnumerator WaitForTap () {
		while (true) {
			if (InputHandler.Instance.inputSignalDown) break;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator WaitForSwap () {
		while (true) {
			if (colorSelector.Current() == Constants.ChooseColor(0)) break;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator WaitForCenterSelect () {
		PlayerActions pa = GameObject.Find ("PlayerActions").GetComponent<PlayerActions> ();
		while (true) {
			if (pa.selected == origin) break;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator WaitForMoveCenter () {
		disabled = true;
		PlayerActions pa = GameObject.Find ("PlayerActions").GetComponent<PlayerActions> ();
		while (pa.selected == origin) {
			disabled = false;
			if (flooded || destroyed)
				break;
			yield return new WaitForEndOfFrame();
		}
	}

	IEnumerator WaitForMove () {
		while (!flooded && !destroyed) {
			yield return new WaitForEndOfFrame();
		}
	}

}

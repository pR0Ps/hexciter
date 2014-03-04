using UnityEngine;
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
	int moves = 30;
	public TextMesh scoreTextMesh;
	public TextMesh movesTextMesh;
	public TextMesh gameOverTextMesh;

	GameObject actionChooser;

	float actionChooserTime;
	GridPlace selected;
	const float OFFSET = 0.8f;
	const float TAPTIME = 0.5f;
	const float SWIPETIME = 1.5f;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
		actionChooser = GameObject.Find("ActionChooser");
	}

	void Start () {
		gameover = false;
		ColorSelector.Instance.Init ();
		movesTextMesh.text = moves.ToString("N0");
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	public void Select (GridPlace gp) {
		if (selected == gp) {
			Deselect ();
			return;
		}
		selected = gp;
		if (!selected.busy && selected.alive) {
			ActionChooser.Instance.Show();
			actionChooserTime = Time.time;
			actionChooser.transform.position = selected.transform.position - Vector3.forward * 5;
		}
	}

	public void Deselect () {
		selected = null;
		if (ActionChooser.Instance.Visible)
			ActionChooser.Instance.Hide ();
	}

	public void Flood () {
		if (selected.Fill (ColorSelector.Instance.Current())) {
			DoMove ();
		}
	}
	
	public void Destroy () {
		int multiplier = 1;
		if (ColorSelector.Instance.Current () == selected.hexaCube.hexColor)
				multiplier = 2;
		int tally = selected.TallyScore(selected.hexaCube.hexColor);
		int earnedScore = tally * multiplier * 100;
		score += earnedScore;
		scoreTextMesh.text = score.ToString ("N0");
		selected.Kill ();
		Debug.Log (tally);
		ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (earnedScore, selected.transform.position);
		DoMove ();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves --;
		movesTextMesh.text = moves.ToString("N0");
		ColorSelector.Instance.NewColor();
	}
	
	void Update () {

		if (moves <= 0) {
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

		if (InputHandler.Instance.inputSignalUp) {
			if (ActionChooser.Instance.Visible) {
				Vector2 diff = (Vector2)actionChooser.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVector);
				if (diff.magnitude >= OFFSET || Time.time - actionChooserTime > TAPTIME) {
					//Action was a drag - perform action based on up position
					if (diff.magnitude >= OFFSET && Time.time - actionChooserTime < SWIPETIME){
						if (diff.x > 0)
							Flood();
						else
							Destroy();
					}
					Deselect();
				}
			}
		}
	}
}

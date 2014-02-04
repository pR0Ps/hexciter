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
		selected.Fill(ColorSelector.Instance.Current());
	}
	
	public void Destroy () {
		int multiplier = 1;
		if (ColorSelector.Instance.Current() == selected.hexaCube.hexColor)
			multiplier = 2;
		// Score = bonus multiplier * number of hexes in the chain * 100
		score += selected.TallyScore(selected.hexaCube.hexColor) * multiplier * 100;
		scoreTextMesh.text = score.ToString("N0");
		selected.Kill();
	}
	
	void Update () {

		if (Input.GetKeyDown(KeyCode.R))
			Application.LoadLevel("main");

		if (moves <= 0) {
			if (!gameover){
				gameover = true;
				gameOverTextMesh.gameObject.SetActive(true);

				#if (UNITY_IPHONE || UNITY_ANDROID)
				SocialManager.Instance.PostScore(score);
				SocialManager.Instance.ShowLeaderboards();
				#endif
			}

			if (InputHandler.Instance.inputSignalDown)
				Application.LoadLevel("main");
			return;
		}

		if (InputHandler.Instance.inputSignalUp) {
			if (ActionChooser.Instance.Visible) {
				Vector2 diff = (Vector2)actionChooser.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVector);
				if (diff.magnitude >= OFFSET || Time.time - actionChooserTime > TAPTIME) {
					//Action was a drag - perform action based on up position
					if (diff.magnitude >= OFFSET && Time.time - actionChooserTime < SWIPETIME){
						moves --;
						if (diff.x > 0)
							Flood();
						else
							Destroy();
						//Made a move, update count and color chooser
						movesTextMesh.text = moves.ToString("N0");
						ColorSelector.Instance.NewColor();
					}
					Deselect();
				}
			}
		}
	}
}

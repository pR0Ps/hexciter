using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {

	public static GridLogic Instance;

	List<GridPlace> gridPlaces = new List<GridPlace>();
	GridPlace origin;
	GridPlace northWestCorner;
	bool spawned;

	public AnimationCurve ScoreCurve;
	int score;
	int moves = 30;
	public TextMesh scoreTextMesh;
	public TextMesh movesTextMesh;
	public TextMesh gameOverTextMesh;

	HexColors selectedColor;
	HexColors nextColor;
	bool flood = true;

	GameObject actionChooser;
	bool actionChooserVisible;
	GridPlace selected;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
		actionChooser = GameObject.Find("ActionChooser");
	}

	void Start () {
		selectedColor = (HexColors)Random.Range(0, 6);
		nextColor = (HexColors)Random.Range(0, 6);

		ColorSelector.Instance.Init (selectedColor, nextColor);
		movesTextMesh.text = moves.ToString("N0");

		gridPlaces = GetComponentsInChildren<GridPlace>().ToList();
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	public void Select (GridPlace gp) {
		selected = gp;
		
		if (!selected.busy && selected.alive) {
			actionChooserVisible = true;
			actionChooser.transform.position = selected.transform.position - Vector3.forward * 5;
			actionChooser.animation.Play ("show");
		}
	}
	
	void Update () {

		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel("main");
		}

		if (moves <= 0) {
			gameOverTextMesh.gameObject.SetActive(true);
			if (InputHandler.Instance.inputSignalDown)
				Application.LoadLevel("main");
			return;
		}

		if (InputHandler.Instance.inputSignalUp) {
			Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVector));
			if (col) {
				if (col.name == "capture")
					flood = true;
				else if (col.name == "destroy")
					flood = false;
				else {
					if (actionChooserVisible)
						actionChooser.animation.Play ("hide");
					actionChooserVisible = false;
					return;
				}
				if (flood) {
					moves --;
					selected.Fill(selectedColor);
				}
				else {
					moves --;
					int multiplier = 1;
					if (selectedColor == selected.hexaCube.hexColor)
						multiplier = 2;

					int scoredThisKill = Integrate(selected.TallyScore(selected.hexaCube.hexColor)) * multiplier;
					score += scoredThisKill;

					scoreTextMesh.text = score.ToString("N0");

					selected.Kill ();
				}
				movesTextMesh.text = moves.ToString("N0");
				selectedColor = nextColor;
				nextColor = (HexColors)Random.Range(0,6);
				ColorSelector.Instance.Swap(nextColor);

				actionChooser.animation.Play ("hide");
				actionChooserVisible = false;
			}
		}
	}

	int Integrate (int x) {
		int total = 0;
		for (int i = 1; i <= x; i ++) {
			total += (int)ScoreCurve.Evaluate(i);
		}
		return total;
	}

}

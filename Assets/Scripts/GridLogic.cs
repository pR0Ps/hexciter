using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {
	
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

	public ColorSelector colorSelector;

	HexColors selectedColor;
	HexColors nextColor;
	bool flood = true;

	GameObject actionChooser;
	bool actionChooserVisible;
	GridPlace selected;

	void Awake () {
		Application.targetFrameRate = 60;
		actionChooser = GameObject.Find("ActionChooser");
	}

	void Start () {
		selectedColor = (HexColors)Random.Range(0, 6);
		nextColor = (HexColors)Random.Range(0, 6);

		colorSelector.Init (selectedColor, nextColor);
		movesTextMesh.text = moves.ToString("N0");

		gridPlaces = GetComponentsInChildren<GridPlace>().ToList();
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.R)) {
			Application.LoadLevel("main");
		}

		if (moves <= 0) {
			gameOverTextMesh.gameObject.SetActive(true);
			if (InputHandler.GetInstance().inputSignalDown)
				Application.LoadLevel("main");
			return;
		}

		if (InputHandler.GetInstance().inputSignalDown) {
			Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(InputHandler.GetInstance().inputVector));
			if (col) {
				selected = col.GetComponent<GridPlace>();

				if (selected && !selected.busy && selected.alive) {
					actionChooserVisible = true;
					actionChooser.transform.position = selected.transform.position - Vector3.forward * 5;
					actionChooser.animation.Play ("show");
				}
			}
		}

		if (InputHandler.GetInstance().inputSignalUp) {
			Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(InputHandler.GetInstance().inputVector));
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
				colorSelector.Swap(nextColor);

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

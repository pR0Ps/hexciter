using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GridLogic : MonoBehaviour {

	public static GridLogic Instance;

	GridPlace origin;
	GridPlace northWestCorner;
	bool spawned;

	public AnimationCurve ScoreCurve;
	int score;
	int moves = 30;
	public TextMesh scoreTextMesh;
	public TextMesh movesTextMesh;
	public TextMesh gameOverTextMesh;

	GameObject actionChooser;
	bool actionChooserVisible;
	float actionChooserTime;
	GridPlace selected;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
		actionChooser = GameObject.Find("ActionChooser");
	}

	void Start () {
		ColorSelector.Instance.Init ();
		movesTextMesh.text = moves.ToString("N0");
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	public void Select (GridPlace gp) {

		selected = gp;
		
		if (!selected.busy && selected.alive) {
			actionChooserVisible = true;
			actionChooserTime = Time.time;
			actionChooser.transform.position = selected.transform.position - Vector3.forward * 5;
			actionChooser.animation.Play ("show");
		}
	}
	
	void Update () {

		HexColors selectedColor = ColorSelector.Instance.Current();

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
			if (actionChooserVisible) {

				Vector2 diff = (Vector2)actionChooser.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVector);
				float OFFSET = 0.8f;
				float TAPTIME = 0.5f;

				if (diff.magnitude < OFFSET && Time.time - actionChooserTime < TAPTIME) {
					//Action was a tap - don't remove the action chooser
					Debug.Log("TAP");
				}
				else {
					Debug.Log("SLIIIIDE");
					//Action was a drag - perform action based on up position

					if (diff.magnitude > OFFSET){
						//capture
						if (diff.x > 0){
							moves--;
							selected.Fill(selectedColor);
						}
						else {
							moves --;
							int multiplier = 1;
							if (selectedColor == selected.hexaCube.hexColor)
								multiplier = 2;
							
							score += Integrate(selected.TallyScore(selected.hexaCube.hexColor)) * multiplier;
							
							scoreTextMesh.text = score.ToString("N0");
							
							selected.Kill();
						}
						
						//Made a move, update count and color chooser
						movesTextMesh.text = moves.ToString("N0");
						ColorSelector.Instance.NewColor();
					}

					//Hide actionchoser
					actionChooser.animation.Play ("hide");
					actionChooserVisible = false;
				}
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

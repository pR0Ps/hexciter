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

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
	}

	void Start () {
		gameover = false;
		ColorSelector.Instance.Init ();
		movesTextMesh.text = moves.ToString("N0");
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	public void Flood(GridPlace start) {
		if (start.Fill (ColorSelector.Instance.Current())) {
			DoMove ();
		}
	}
	
	public void Destroy (GridPlace start) {
		if (start.Kill()) {
			int multiplier = 1;
			if (ColorSelector.Instance.Current() == start.hexaCube.hexColor){
				multiplier = 2;
			}

			// Score = bonus multiplier * number of hexes in the chain * 100
			score += start.TallyScore(start.hexaCube.hexColor) * multiplier * 100;
			scoreTextMesh.text = score.ToString ("N0");
			DoMove ();
		}
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
	}
}

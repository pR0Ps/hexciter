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
	int level;
	int moves;
	int maxmoves;
	int minscore;
	public TextMesh scoreTextMesh;
	public TextMesh targetTextMesh;
	public TextMesh gameOverTextMesh;

	void Awake () {
		// tell the game to run at 60 fps, maybe put this some where better later
		Application.targetFrameRate = 60;
		Instance = this;
	}

	void Start () {
		gameover = false;
		level = 1;
		ColorSelector.Instance.Init ();
		
		origin = transform.FindChild("Origin").GetComponent<GridPlace>();
		origin.SlowSpawn();
	}

	void UpdateUI(){
		ProgressBar.Instance.SetPercent(moves/(float)maxmoves);
		targetTextMesh.text = minscore.ToString("N0");
	}

	void NextLevel(){
		level++;
		maxmoves = ProgressBar.NUM_HEXES;
		moves = 0;
		minscore = score + level * 2000;
		UpdateUI();
	}

	//Generator that yields lists of GridPlaces by increasing depth from the origin
	//Returns the origin first, then a list of all it's siblings,
	//then all their siblings, etc.
	public IEnumerable<GridPlace[]> GetSiblings(GridPlace gp) {
		Dictionary<GridPlace,int> seen = new Dictionary<GridPlace,int>();
		Queue<GridPlace> queue = new Queue<GridPlace>();

		int currDepth = -1;

		//Add the start node
		queue.Enqueue(gp);
		seen.Add(gp, 0);

		//BFS
		while (queue.Count > 0){
			//Check the depth of the oldest item in the queue
			int depth = seen[queue.Peek()];
			if (depth > currDepth){
				//If the depth is greater than the current depth it means that all
				//items of the previous depth have been removed from the queue
				//What's left will be all the items at currDepth + 1
				currDepth = depth;
				yield return queue.ToArray();
			}

			//Take the oldest item out of the queue and add it's children the to the
			//queue at depth + 1 (if they haven't already been added)
			GridPlace temp = queue.Dequeue();
			List<GridPlace> siblings = temp.sibs.ExistingSibs();
			for (int i = 0; i < siblings.Count; i ++) {
				if (!seen.ContainsKey(siblings[i])){
					seen.Add(siblings[i], depth + 1);
					queue.Enqueue(siblings[i]);
				}
			}
		}
	}

	public void Flood(GridPlace start) {
		if (start.Fill (ColorSelector.Instance.Current())) {
			DoMove();
		}
	}

	public void Destroy (GridPlace start) {
		int multiplier = 1;
		if (ColorSelector.Instance.Current () == start.hexaCube.hexColor) multiplier = 2;

		int tally = start.TallyScore(start.hexaCube.hexColor);
		int earnedScore = tally * multiplier * 100;

		score += earnedScore;
		scoreTextMesh.text = score.ToString("N0");
		start.Kill ();

		ObjectPoolManager.Instance.Pop("ScorePopup").GetComponent<ScorePopup>().Show (earnedScore, start.transform.position);
		DoMove ();
	}

	//Anytyime a move is made, this is called
	public void DoMove(){
		//Update count and color chooser
		moves++;
		UpdateUI();
		ColorSelector.Instance.NewColor();
	}
	
	void Update () {

		if (moves >= maxmoves && score < minscore) {
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
		else if (moves >= maxmoves){
			NextLevel();
		}
	}
}

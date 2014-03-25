using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour {

	private GridLogic gridLogic;

	public bool swiping { get; private set; }
	public GridPlace selected { get; private set; }

	const float OFFSET = 1.7f;

	void Awake(){
		gridLogic = GameObject.Find("Grid").GetComponent<GridLogic>();
	}
	
	void Start () {
		swiping = false;
		selected = null;
	}

	public void Deselect(){
		swiping = false;
		if (selected != null){
			Utils.ScaleSiblings(selected, false);
		}
		selected = null;
	}

	public void Destroy(){
		if (selected != null && !selected.reserved) {
			gridLogic.Destroy(selected);
		}
		Deselect();
	}

	public void Flood(){
		if (selected != null && !selected.reserved){
			gridLogic.Flood(selected);
		}
		Deselect();
	}

	public void DownAction(GridPlace gp){
		if (gp == null) return;

		if (!swiping && !gp.busy && gp.alive) {
			swiping = true;
			selected = gp;
			Utils.ScaleSiblings(selected, true);
		}
	}

	public void UpAction(){
		if (swiping) {
			// not the same place, do swiping action
			Vector2 diff = (Vector2)selected.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVectorScreen);
			if (Mathf.Abs(diff.x) >= OFFSET) {
				//Action was a drag - perform action based on up positio
				if (diff.x > 0)
					Flood();
				else
					Destroy();
			}
		}
		Deselect();
	}

	public void Update(){
		if (swiping) {
			Vector2 diff = (Vector2)selected.transform.position - (Vector2)InputHandler.Instance.inputVectorWorld;
			IconController.Instance.SetTarget (Mathf.Clamp(diff.x/8.7f, -1, 1));
		}
		else
			IconController.Instance.SetInvisible ();

		if (swiping && InputHandler.Instance.inputSignalUp){
			UpAction();
		}
	}
}

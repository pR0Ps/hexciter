using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour {

	private GridLogic gridLogic;
	private IconController iconController;

	public bool swiping { get; private set; }
	public GridPlace selected { get; private set; }

	const float OFFSET = 1.7f;

	void Awake(){
		gridLogic = GameObject.Find("Grid").GetComponent<GridLogic>();
		iconController = GameObject.Find("Icons").GetComponent<IconController>();
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

	bool validPlace(GridPlace gp){
		return gp != null && !gp.busy && gp.hexaCube.alive;
	}

	public void Destroy(){
		if (validPlace(selected)) {
			gridLogic.Destroy(selected);
		}
		Deselect();
	}

	public void Flood(){
		if (validPlace(selected)){
			gridLogic.Flood(selected);
		}
		Deselect();
	}

	public void DownAction(GridPlace gp){
		if (!swiping && validPlace(gp)) {
			swiping = true;
			selected = gp;
			Utils.ScaleSiblings(selected, true);
		}
	}

	public void UpAction(){
		if (!gridLogic.disabled && swiping) {
			// not the same place, do swiping action
			Vector2 diff = (Vector2)selected.transform.position - (Vector2)Camera.main.ScreenToWorldPoint(InputHandler.Instance.inputVectorScreen);
			if (Mathf.Abs(diff.x) >= OFFSET) {
				//Action was a long enough drag, do the action
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
			iconController.SetTarget (Mathf.Clamp(diff.x/8.7f, -1, 1));
		}
		else
			iconController.SetInvisible ();

		if (swiping && InputHandler.Instance.inputSignalUp){
			UpAction();
		}
	}
}

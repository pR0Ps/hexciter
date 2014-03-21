using UnityEngine;
using System.Collections;

public class ScoreBar : MonoBehaviour {

	HexaCube[] progressCubes = new HexaCube[32];
	bool[] activeCubes = new bool[32];

	void Start () {
		for (int i=0; i<32; i++) {
			progressCubes[i] = transform.FindChild(i.ToString()).GetComponent<HexaCube>();
			progressCubes[i].Spawn(Constants.HexColors.Black);
			progressCubes[i].Despawn();
		}
	}

	public void ReportProgress (int currentScore, int startScore, int endScore) {
		int progress = (int)(Mathf.Lerp(0, 1, (float)(currentScore - startScore)/(float)(endScore - startScore)) * 32);
		for (int i=0; i<32; i++) {
			if (!activeCubes[i]) {
				StopAllCoroutines();
				if (progress < i) { // reporting progess decrease (ie, new level)
					StartCoroutine(Despawn(progress, 32));
					StartCoroutine(Flood (i, 32));
				}

				StartCoroutine(Flood (i, progress));
				return;
			}
		}
	}
	//Testing Stuff
	/*
	public int cur;
	public int start;
	public int end;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			ReportProgress(cur, start, end);
		}
	}
	*/

	public IEnumerator Despawn (int startIndex, int endIndex) {
		for (int i=startIndex; i<endIndex; i++) {
			activeCubes[i] = false;
			progressCubes[i].Despawn();
			yield return new WaitForSeconds(0.05f);
		}
	}

	public IEnumerator Flood (int startIndex, int endIndex) {
		for (int i=startIndex; i<endIndex; i++) {
			activeCubes[i] = true;
			progressCubes[i].Spawn(Constants.HexColors.White);
			yield return new WaitForSeconds(0.05f);
		}
	}
}

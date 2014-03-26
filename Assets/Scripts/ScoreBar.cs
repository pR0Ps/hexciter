using UnityEngine;
using System.Collections;

public class ScoreBar : MonoBehaviour {

	const int NUM_CUBES = 32;
	HexaCube[] progressCubes = new HexaCube[NUM_CUBES];
	bool[] activeCubes = new bool[NUM_CUBES];
	int progress;

	void Start () {
		for (int i=0; i<NUM_CUBES; i++) {
			progressCubes[i] = transform.FindChild(i.ToString()).GetComponent<HexaCube>();
			progressCubes[i].Spawn(Constants.HEX_BLACK);
			progressCubes[i].Despawn();
		}
	}

	public void ReportProgress (int currentScore, int startScore, int endScore) {
		progress = (int)(Mathf.Lerp(0, 1, (float)(currentScore - startScore)/(float)(endScore - startScore)) * NUM_CUBES);
		for (int i=0; i<NUM_CUBES; i++) {
			if (!activeCubes[i]) {
				//StopAllCoroutines();
				if (progress < i) { // reporting progess decrease (ie, new level)
					StartCoroutine(Despawn(progress, NUM_CUBES));
					StartCoroutine(Flood (i, NUM_CUBES));
				}
				StartCoroutine(Flood (i, progress));
				return;
			}
		}
	}

	public IEnumerator Despawn (int startIndex, int endIndex) {
		yield return new WaitForSeconds(0.5f);
		for (int i=startIndex; i<endIndex; i++) {
			activeCubes[i] = false;
			progressCubes[i].Despawn();
			yield return new WaitForSeconds(0.05f);
		}
	}

	public IEnumerator Flood (int startIndex, int endIndex) {
		for (int i=startIndex; i<endIndex; i++) {
			activeCubes[i] = true;
			progressCubes[i].Spawn(Constants.HEX_WHITE);
			yield return new WaitForSeconds(0.05f);
		}
	}

	public int HexesRemaining(){
		return NUM_CUBES - progress;
	}
}

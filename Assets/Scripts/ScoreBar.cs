using UnityEngine;
using System.Collections;

public class ScoreBar : MonoBehaviour {

	public const int NUM_CUBES = 32;
	private int progress;
	HexaCube[] progressCubes = new HexaCube[NUM_CUBES];
	bool[] activeCubes = new bool[NUM_CUBES];

	void Start () {
		for (int i=0; i<NUM_CUBES; i++) {
			progressCubes[i] = transform.FindChild(i.ToString()).GetComponent<HexaCube>();
			progressCubes[i].Spawn(Constants.HEX_BLACK);
			progressCubes[i].Despawn();
			progressCubes[i].gameObject.SetActive(false);
		}
		StartCoroutine (StartupAnimation());
	}

	IEnumerator StartupAnimation () {

		for (int i=0; i<NUM_CUBES; i++) {
			HexaCube cube = transform.FindChild("startupAnimation/"+i.ToString()).GetComponent<HexaCube>();
			cube.Spawn(Constants.HEX_BLACK);
			cube.transform.FindChild("LookRotation").localScale = Vector3.one * 1.3f;
			yield return new WaitForSeconds(0.02f);
		}
		yield return new WaitForSeconds (0.5f);

		transform.FindChild ("startupAnimation").gameObject.SetActive (false);
		transform.FindChild ("progressBar").gameObject.SetActive (true);
		transform.FindChild ("progressBarBG").gameObject.SetActive (true);

		for (int i=0; i<NUM_CUBES; i++)
			progressCubes[i].gameObject.SetActive(true);

	}

	public void ReportProgress (int currentScore, int startScore, int endScore) {
		progress = (int)(Mathf.Lerp(0, 1, (float)(currentScore - startScore)/(float)(endScore - startScore)) * NUM_CUBES);
		for (int i=0; i<NUM_CUBES; i++) {
			if (!activeCubes[i] || progress == NUM_CUBES) {
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
		yield return new WaitForSeconds(0.7f);
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

	public int NumBlackHexes () {
		return NUM_CUBES - progress;
	}

}

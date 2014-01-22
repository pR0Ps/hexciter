using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum HexColors {
	Blue,
	Orange,
	Purple,
	Green,
	Yellow,
	Red
}

public class HexaCube : InteractableObject {

	public List<Color> colors;
	public HexColors hexColor;

	public GridPlace gridPlace;
	bool rippling;

	VertexColor vertexColor;

	IEnumerator ColorLerp (HexColors newHexColor) {
		Color newColor = colors[(int)newHexColor];

		float t = 0;
		while (vertexColor.vColor != newColor) {
			t += Time.deltaTime;
			vertexColor.UpdateColor(Color.Lerp(vertexColor.vColor, newColor, t * 2.5f));
			yield return new WaitForEndOfFrame();
		}
	}

	//A public method to tell a cube to lerp to a new color
	//Currently used by the color selector
	public void GUIColorLerp (HexColors newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
	}

	public void Fill (HexColors newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
		animation.Play("Wiggle");
	}

	void Awake () {
		hexColor = (HexColors)Random.Range(0, 6);
		vertexColor = GetComponentInChildren<VertexColor>();
	}

	void RandomizeColor () {
		hexColor = (HexColors)Random.Range(0, 6);
		vertexColor.UpdateColor(colors[(int)hexColor]);
	}
	
	public void Spawn () {
		animation.Play("Spawn");
		gridPlace.busy = true;
		RandomizeColor();
	}

	public void SlowSpawn () {
		animation.Play("SlowSpawn");
		RandomizeColor();
	}

	public void Kill () {
		StartCoroutine(KillCo());
	}

	IEnumerator KillCo () {
		Despawn ();
		while(gridPlace.busy) {
			yield return new WaitForEndOfFrame();
		}
		Spawn ();
	}
	
	public void Despawn () {
		animation.Play("Despawn");
		gridPlace.busy = true;
	}

	void SpawnedCallback () {
		gridPlace.busy = false;
		gridPlace.alive = true;
	}

	void DespawnedCallback () {
		gridPlace.busy = false;
		gridPlace.alive = true;
	}

	void WiggleCallback () {
		gridPlace.busy = false;
		gridPlace.alive = true;
	}
}

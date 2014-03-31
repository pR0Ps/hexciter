using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexaCube : InteractiveObject {
	
	public int hexColor;
	public GridPlace gridPlace;
	public bool alive { get; private set; }

	VertexColor vertexColor;
	
	void Awake () {
		vertexColor = GetComponentInChildren<VertexColor>();
		alive = true;
	}

	void setBusy(bool b){
		if (gridPlace) {
			gridPlace.busy = b;
		}
	}
	
	//A public method to tell a cube to lerp to a new color
	//Currently used by the color selector
	public void GUIColorLerp (int newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
	}
	
	IEnumerator ColorLerp (int newHexColor) {
		Color newColor = Constants.HEX_COLORS[newHexColor];
		Color initialColor = vertexColor.vColor;
		float t = 0;
		while (vertexColor.vColor != newColor) {
			t += Time.deltaTime * 2;
			vertexColor.UpdateColor(Color.Lerp(initialColor, newColor, t));
			yield return new WaitForEndOfFrame();
		}
	}

	public void Fill (int newHexColor) {
		setBusy(true);
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
		animation.Play("Wiggle");
	}

	public void Kill () {
		StartCoroutine(KillCo());
	}
	
	IEnumerator KillCo () {
		Despawn();
		while(alive)
			yield return new WaitForEndOfFrame();
		Spawn();
	}

	public void Spawn () {
		Spawn (Random.Range (0, Constants.NUMBER_OF_COLORS));
	}

	public void Spawn (int newColor) {
		DoSpawn("Spawn", newColor);
	}

	public void SlowSpawn () {
		SlowSpawn (Random.Range (0, Constants.NUMBER_OF_COLORS));
	}

	public void SlowSpawn (int newColor) {
		DoSpawn("SlowSpawn", newColor);
	}

	private void DoSpawn(string anim, int color){
		setBusy(true);
		alive = true;
		hexColor = color;
		vertexColor.UpdateColor(Constants.HEX_COLORS[hexColor]);
		animation.Play(anim);
	}
	
	public void Despawn () {
		setBusy(true);
		animation.Play("Despawn");
	}
	
	void SpawnedCallback () {
		setBusy(false);
	}
	
	void DespawnedCallback () {
		setBusy(false);
		alive = false;
	}
	
	void WiggleCallback () {
		setBusy(false);
	}
}
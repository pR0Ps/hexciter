using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexaCube : InteractiveObject {
	
	public int hexColor;
	public GridPlace gridPlace;

	VertexColor vertexColor;
	
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
	
	//A public method to tell a cube to lerp to a new color
	//Currently used by the color selector
	public void GUIColorLerp (int newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
	}
	
	public void Fill (int newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
		animation.Play("Wiggle");
	}
	
	void Awake () {
		vertexColor = GetComponentInChildren<VertexColor> ();
	}
	
	void RandomizeColor () {
		hexColor = Random.Range(0, Constants.NUMBER_OF_COLORS);
		vertexColor.UpdateColor(Constants.HEX_COLORS[hexColor]);
	}

	public void Spawn () {
		animation.Play("Spawn");
		RandomizeColor();
	}

	public void Spawn (int newColor) {
		animation.Play("Spawn");
		hexColor = newColor;
		vertexColor.UpdateColor(Constants.HEX_COLORS[hexColor]);
	}
	
	public void SlowSpawn () {
		animation.Play("SlowSpawn");
		RandomizeColor();
	}
	
	public void SlowSpawn (int newColor) {
		animation.Play("SlowSpawn");
		hexColor = newColor;
		vertexColor.UpdateColor(Constants.HEX_COLORS[hexColor]);
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
		if (!gridPlace)
			return;
		gridPlace.busy = true;
	}
	
	void SpawnedCallback () {
		if (!gridPlace)
			return;
		gridPlace.busy = false;
		gridPlace.alive = true;
		gridPlace.reserved = false;
	}
	
	void DespawnedCallback () {
		if (!gridPlace)
			return;
		gridPlace.busy = false;
		gridPlace.alive = true;
	}
	
	void WiggleCallback () {
		if (!gridPlace)
			return;
		gridPlace.busy = false;
		gridPlace.alive = true;
	}
}
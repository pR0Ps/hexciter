using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexaCube : InteractiveObject {
	
	public Color hexColor;
	public GridPlace gridPlace;
	public bool alive { get; private set; }
	public bool busy { get; private set; }

	VertexColor vertexColor;
	
	void Awake () {
		vertexColor = GetComponentInChildren<VertexColor>();
		alive = false;
		busy = false;
	}
	
	//A public method to tell a cube to lerp to a new color
	//Currently used by the color selector
	public void GUIColorLerp (Color newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
	}
	
	IEnumerator ColorLerp (Color newColor) {
		Color initialColor = vertexColor.vColor;
		float t = 0;
		while (vertexColor.vColor != newColor) {
			t += Time.deltaTime * 2;
			vertexColor.UpdateColor(Color.Lerp(initialColor, newColor, t));
			yield return new WaitForEndOfFrame();
		}
	}

	public void Fill (Color newHexColor) {
		busy = true;
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
		animation.Play("Wiggle");
	}

	public void Kill () {
		StartCoroutine(KillCo(Constants.RandomColor()));
	}

	public void Kill (Color color) {
		StartCoroutine(KillCo(color));
	}

	IEnumerator KillCo (Color color) {
		Despawn();
		while(alive)
			yield return new WaitForEndOfFrame();
		Spawn(color);
	}

	public void Spawn () {
		Spawn (Constants.RandomColor());
	}

	public void Spawn (Color newColor) {
		DoSpawn("Spawn", newColor);
	}

	public void SlowSpawn () {
		SlowSpawn (Constants.RandomColor());
	}

	public void SlowSpawn (Color newColor) {
		DoSpawn("SlowSpawn", newColor);
	}

	private void DoSpawn(string anim, Color color){
		busy = true;
		alive = true;
		hexColor = color;
		vertexColor.UpdateColor(hexColor);
		animation.Play(anim);
	}
	
	public void Despawn () {
		busy = true;
		animation.Play("Despawn");
	}
	
	void SpawnedCallback () {
		busy = false;
	}
	
	void DespawnedCallback () {
		busy = false;
		alive = false;
	}
	
	void WiggleCallback () {
		busy = false;
	}
}
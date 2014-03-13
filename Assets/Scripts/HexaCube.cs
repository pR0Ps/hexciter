using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexaCube : InteractiveObject {
	
	public Constants.HexColors hexColor;
	public GridPlace gridPlace;

	// used for the looking-at-finger effect
	bool lookRotationEnabled = false;
	Transform lookTransform;
	Quaternion targetRotation;
	float lookDistance = 30.0f; // the closer this is to 0, the more intense the look effect
	float lookSpeed = 10.0f;

	VertexColor vertexColor;
	
	IEnumerator ColorLerp (Constants.HexColors newHexColor) {
		Color newColor = Constants.Colors[(int)newHexColor];
		Color initialColor = vertexColor.vColor;
		float t = 0;
		while (vertexColor.vColor != newColor) {
			t += Time.deltaTime * 2;
			vertexColor.UpdateColor(Color.Lerp(initialColor, newColor, t));
			yield return new WaitForEndOfFrame();
		}
	}
	
	// only hex's belonging to a grid place should call this
	// ie. we don't want other hexacubes fucking up
	public void EnableLookRotation () {
		lookRotationEnabled = true;
	}

	void Update () {
		if (lookRotationEnabled) {
			if (PlayerActions.Instance.swiping) {
				Vector3 lookPoint = new Vector3 (InputHandler.Instance.inputVectorWorld.x, InputHandler.Instance.inputVectorWorld.y, lookDistance);
				targetRotation = Quaternion.LookRotation(lookPoint - lookTransform.position);
			}
			else
				targetRotation = Quaternion.identity;

			lookTransform.rotation = Quaternion.Slerp (lookTransform.rotation, targetRotation, lookSpeed * Time.deltaTime);
		}
	}
	
	//A public method to tell a cube to lerp to a new color
	//Currently used by the color selector
	public void GUIColorLerp (Constants.HexColors newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
	}
	
	public void Fill (Constants.HexColors newHexColor) {
		hexColor = newHexColor;
		StartCoroutine(ColorLerp(newHexColor));
		animation.Play("Wiggle");
	}
	
	void Awake () {
		vertexColor = GetComponentInChildren<VertexColor> ();
		lookTransform = transform.FindChild ("LookRotation");
	}
	
	void RandomizeColor () {
		hexColor = (Constants.HexColors)Random.Range(0, Constants.NUMBER_OF_COLORS);
		vertexColor.UpdateColor(Constants.Colors[(int)hexColor]);
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
	
	public void SlowSpawn (Constants.HexColors newColor) {
		animation.Play("SlowSpawn");
		hexColor = newColor;
		vertexColor.UpdateColor(Constants.Colors[(int)hexColor]);
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
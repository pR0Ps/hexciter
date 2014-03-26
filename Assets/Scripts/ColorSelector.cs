using UnityEngine;
using System.Collections;

public class ColorSelector : InteractiveObject {
	
	HexaCube hex1;
	HexaCube hex2;
	Animation anim;
	
	bool hexOneInFront = true;
	
	void Awake () {
		hex1 = transform.FindChild("rotator/HexaCube1").GetComponent<HexaCube>();
		hex2 = transform.FindChild("rotator/HexaCube2").GetComponent<HexaCube>();
		anim = GetComponentInChildren<Animation>();
	}
	
	public void Init () {
		hex1.GUIColorLerp((Constants.HexColors)Random.Range(0, Constants.NUMBER_OF_COLORS));
		hex2.GUIColorLerp((Constants.HexColors)Random.Range(0, Constants.NUMBER_OF_COLORS));
	}
	
	//When tapped, swap the cubes
	public override void TapAction () {
		Swap ();
	}
	
	//Swap the hexes around
	public void Swap(){
		if (hexOneInFront) {
			hexOneInFront = false;
			anim.Play ("Flip1");
		}
		else {
			hexOneInFront = true;
			anim.Play ("Flip2");
		}
	}
	
	//Use the front color: Add a new color to the front and play the swap animation
	public void NewColor(){
		//Change the color
		if (hexOneInFront)
			hex1.GUIColorLerp((Constants.HexColors)Random.Range(0, Constants.NUMBER_OF_COLORS));
		else
			hex2.GUIColorLerp((Constants.HexColors)Random.Range(0, Constants.NUMBER_OF_COLORS));
		
		//Start the swap animation
		Swap ();
	}
	
	//Get the current color in front
	public Constants.HexColors Current (){
		if (hexOneInFront)
			return hex1.hexColor;
		else
			return hex2.hexColor;
	}
}
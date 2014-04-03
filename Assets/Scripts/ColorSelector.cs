using UnityEngine;
using System.Collections;

public class ColorSelector : InteractiveObject {
	
	HexaCube hex1;
	HexaCube hex2;
	Animation anim;
	
	bool hexOneInFront = true;
	bool canSwap = true;
	
	void Awake () {
		hex1 = transform.FindChild("rotator/HexaCube1").GetComponent<HexaCube>();
		hex2 = transform.FindChild("rotator/HexaCube2").GetComponent<HexaCube>();
		anim = transform.FindChild("rotator").GetComponent<Animation>();
	}
	
	public void Init () {
		hex1.GUIColorLerp(Constants.RandomColor());
		hex2.GUIColorLerp(Constants.RandomColor());
	}

	public void Init (int c1, int c2) {
		hex1.GUIColorLerp(Constants.ChooseColor(c1));
		hex2.GUIColorLerp(Constants.ChooseColor(c2));
	}

	public void DisableSwap () {
		canSwap = false;
	}

	public void EnableSwap () {
		canSwap = true;
	}
	
	//When tapped, swap the cubes
	public override void TapAction () {
		TapSwap ();
	}
	
	//Swap the hexes around
	public void TapSwap(){
		if (!canSwap)
			return;
		if (hexOneInFront) {
			hexOneInFront = false;
			anim.Play ("Flip1");
		}
		else {
			hexOneInFront = true;
			anim.Play ("Flip2");
		}
	}
	
	public void MoveSwap() {
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
			hex1.GUIColorLerp(Constants.RandomColor());
		else
			hex2.GUIColorLerp(Constants.RandomColor());
		
		//Start the swap animation
		MoveSwap ();
	}
	
	public void NewColor(int c){
		//Change the color
		if (hexOneInFront)
			hex1.GUIColorLerp(Constants.ChooseColor(c));
		else
			hex2.GUIColorLerp(Constants.ChooseColor(c));
		
		//Start the swap animation
		MoveSwap ();
	}

	//Get the current color in front
	public Color Current (){
		if (hexOneInFront)
			return hex1.hexColor;
		else
			return hex2.hexColor;
	}
}
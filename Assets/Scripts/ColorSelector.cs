using UnityEngine;
using System.Collections;

public class ColorSelector : InteractableObject {

	HexaCube hex1;
	HexaCube hex2;
	
	bool hexOneInFront = true;

	void Start () {
		hex1 = transform.FindChild("HexaCube1").GetComponent<HexaCube>();
		hex2 = transform.FindChild("HexaCube2").GetComponent<HexaCube>();
	}

	public void Init (HexColors hex1col, HexColors hex2col) {
		hex1.GUIColorLerp(hex1col);
		hex2.GUIColorLerp(hex2col);
	}

	//When tapped, swap the cubes
	public override void TapAction () {
		Swap ();
	}

	//Swap the hexes around
	public void Swap(){
		if (hexOneInFront) {
			hexOneInFront = false;
			animation.Play ("Flip1");
		}
		else {
			hexOneInFront = true;
			animation.Play ("Flip2");
		}
	}

	//Switch the hexes around and lerp the back hex to the new next color
	public void Swap (HexColors newNext) {
		//Change the color
		if (hexOneInFront)
			hex1.GUIColorLerp(newNext);
		else
			hex2.GUIColorLerp(newNext);

		//Start the swap animation
		Swap ();
	}
}

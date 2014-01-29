using UnityEngine;
using System.Collections;

public class ColorSelector : InteractiveObject {

	public static ColorSelector Instance;

	HexaCube hex1;
	HexaCube hex2;
	Animation anim;
	
	bool hexOneInFront = true;

	void Awake () {
		Instance = this;
		hex1 = transform.FindChild("rotator/HexaCube1").GetComponent<HexaCube>();
		hex2 = transform.FindChild("rotator/HexaCube2").GetComponent<HexaCube>();
		anim = GetComponentInChildren<Animation>();
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
			anim.Play ("Flip1");
		}
		else {
			hexOneInFront = true;
			anim.Play ("Flip2");
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

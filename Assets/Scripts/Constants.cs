using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Constants {
	
	public const int NUM_COLORS = 4;
	public const int NUM_COLOR_SCHEMES = 3;

	public static Color32 BLACK = HexToColor("000000");
	public static Color32 WHITE = HexToColor("FFFFFF");
	public static Color32 GREY = HexToColor("808080");

	//Each color scheme is a different array
	public static Color32[][] ALL_COLORS = new Color32[][] {
		new Color32[]{
			HexToColor("E40045"), //red
			HexToColor("FF7600"), //orange
			HexToColor("01939A"), //blue
			HexToColor("67E300"), //green
		},
		new Color32[]{
			HexToColor("00AC6B"), //green
			HexToColor("FFEB00"), //yellow
			HexToColor("560EAD"), //purple
			HexToColor("FF4F00"), //orange
		},
		new Color32[]{
			HexToColor("C9770A"), //purple
			HexToColor("1144AA"), //blue
			HexToColor("FF8700"), //orange
			HexToColor("A5Ef00"), //green
		}
	};

	public static Color32 RandomColor(){
		return ALL_COLORS[0][Random.Range (0, NUM_COLORS)];
	}

	static Color32 HexToColor (string hexadecimal) {
		
		byte r = System.Convert.ToByte (hexadecimal.Substring (0, 2), 16);
		byte g = System.Convert.ToByte (hexadecimal.Substring (2, 2), 16);
		byte b = System.Convert.ToByte (hexadecimal.Substring (4, 2), 16);

		return new Color32 (r, g, b, 255);
	}

	public static float CUBE_LOOK_DIST = 150.0f; // the closer this is to 0, the more intense the look effect
	public static float CUBE_LOOK_SPEED = 10.0f;
}
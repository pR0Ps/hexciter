using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Constants {
	
	public const int NUM_COLORS = 4;
	public const int NUM_COLOR_SCHEMES = 3;

	public static Color32 BLACK = HexToColor32("000000");
	public static Color32 WHITE = HexToColor32("FFFFFF");
	public static Color32 GREY = HexToColor32("808080");

	//Each color scheme is a different array
	public static Color32[][] ALL_COLORS = new Color32[][] {
		new Color32[]{
			HexToColor32("E40045"), //red
			HexToColor32("FF7600"), //orange
			HexToColor32("01939A"), //blue
			HexToColor32("67E300"), //green
		},
		new Color32[]{
			HexToColor32("FF4F00"), //orange
			HexToColor32("FFA700"), //yellow
			HexToColor32("560EAD"), //purple
			HexToColor32("00AC6B"), //green
		},
		new Color32[]{
			HexToColor32("C9007A"), //purple
			HexToColor32("FFA700"), //yellow
			HexToColor32("1144AA"), //blue
			HexToColor32("5DE100"), //green
		}
	};

	public static Color32 ChooseColor(int i){
		return ALL_COLORS[PlayerPrefs.GetInt("color_scheme", 0)][i];
	}

	public static Color32 RandomColor(){
		return ALL_COLORS[PlayerPrefs.GetInt("color_scheme", 0)][Random.Range (0, NUM_COLORS)];
	}

	static Color32 HexToColor32 (string hexadecimal) {
		
		byte r = System.Convert.ToByte (hexadecimal.Substring (0, 2), 16);
		byte g = System.Convert.ToByte (hexadecimal.Substring (2, 2), 16);
		byte b = System.Convert.ToByte (hexadecimal.Substring (4, 2), 16);

		return new Color32 (r, g, b, 255);
	}

	public static float CUBE_LOOK_DIST = 100.0f; // the closer this is to 0, the more intense the look effect
	public static float CUBE_LOOK_SPEED = 10.0f;
}
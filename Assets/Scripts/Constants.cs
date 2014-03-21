using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class Constants {
	
	public const int NUMBER_OF_COLORS = 4;

	public enum HexColors {
		Blue,
		//Orange,
		//Purple,
		Green,
		Yellow,
		Red,
		Grey
	}
	
	public static Color32[] Colors = new Color32[] {
		HexToColor32("E40045"),
		HexToColor32("FF7600"),
		HexToColor32("01939A"),
		HexToColor32("67E300"),
		new Color32 (128, 128, 128, 255)
	};

	static Color32 HexToColor32 (string hexadecimal) {
		
		byte r = Convert.ToByte (hexadecimal.Substring (0, 2), 16);
		byte g = Convert.ToByte (hexadecimal.Substring (2, 2), 16);
		byte b = Convert.ToByte (hexadecimal.Substring (4, 2), 16);

		return new Color32 (r, g, b, 255);
	}

	public static float CUBE_LOOK_DIST = 150.0f; // the closer this is to 0, the more intense the look effect
	public static float CUBE_LOOK_SPEED = 10.0f;
}
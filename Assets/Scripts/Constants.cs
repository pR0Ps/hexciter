using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Constants {
	
	public const int NUMBER_OF_COLORS = 5;
	
	public enum HexColors {
		Blue,
		//Orange,
		Purple,
		Green,
		Yellow,
		Red,
		Grey
	}
	
	public static Color[] Colors = new Color[] {
		new Color (0.206f, 0.728f, 0.801f, 1.0f),
		//new Color (0.801f, 0.523f, 0.171f, 1.0f),
		new Color (0.462f, 0.203f, 0.691f, 1.0f),
		new Color (0.175f, 0.699f, 0.222f, 1.0f),
		new Color (0.868f, 0.772f, 0.236f, 1.0f),
		new Color (0.728f, 0.177f, 0.177f, 1.0f),
		new Color (0.500f, 0.500f, 0.500f, 1.0f),
	};

	public static float CUBE_LOOK_DIST = 150.0f; // the closer this is to 0, the more intense the look effect
	public static float CUBE_LOOK_SPEED = 10.0f;
}
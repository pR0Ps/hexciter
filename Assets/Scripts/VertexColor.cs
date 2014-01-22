using UnityEngine;
using System.Collections;

public class VertexColor : MonoBehaviour {

	public Color vColor;
	Color[] buffer;
	Mesh mesh;


	void Awake () {
		mesh = GetComponent<MeshFilter>().mesh;
		buffer = new Color[mesh.vertexCount];
	}

	public void UpdateColor(Color color) {
		vColor = color;
		for (int i = 0; i < buffer.Length; i++)
			buffer[i] = vColor;

		mesh.colors = buffer;
	}
}

using UnityEngine;
using System.Collections;

public class IconAnimation : MonoBehaviour {

	float visibility;
	float targetVisibility;
	float lerpSpeed = 10f;

	SpriteRenderer[] spriteRenders;

	public void SetVisiblity (float v) {
		targetVisibility = v;
	}

	void Awake () {
		spriteRenders = GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < spriteRenders.Length; i ++)
			spriteRenders[i].color = new Color(1,1,1,0);
	}

	void Start () {
		animation.Play ();
		animation[animation.clip.name].speed = 0;
		animation [animation.clip.name].normalizedTime = 0;
	}

	void Update () {
		visibility = Mathf.Lerp (visibility, targetVisibility, Time.deltaTime * lerpSpeed);
		for (int i = 0; i < spriteRenders.Length; i ++)
			spriteRenders[i].color = Color.Lerp(spriteRenders[i].color, new Color(1,1,1,visibility), Time.deltaTime * lerpSpeed);
		animation[animation.clip.name].normalizedTime = visibility;
	}
}

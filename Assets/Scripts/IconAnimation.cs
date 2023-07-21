using UnityEngine;
using System.Collections;

public class IconAnimation : MonoBehaviour {

	float visibility;
	float targetVisibility;
	float lerpSpeed = 10f;

	SpriteRenderer[] spriteRenders;
	Animation anim;

	public void SetVisiblity (float v) {
		targetVisibility = v;
	}

	void Awake () {
		anim = GetComponent<Animation>();
		spriteRenders = GetComponentsInChildren<SpriteRenderer> ();
		for (int i = 0; i < spriteRenders.Length; i ++)
			spriteRenders[i].color = new Color(1,1,1,0);
	}

	void Start () {
		anim.Play ();
		anim[anim.clip.name].speed = 0;
		anim[anim.clip.name].normalizedTime = 0;
	}

	void Update () {
		visibility = Mathf.Lerp (visibility, targetVisibility, Time.deltaTime * lerpSpeed);
		for (int i = 0; i < spriteRenders.Length; i ++)
			spriteRenders[i].color = Color.Lerp(spriteRenders[i].color, new Color(1,1,1,visibility), Time.deltaTime * lerpSpeed);
		anim[anim.clip.name].normalizedTime = visibility;
	}
}

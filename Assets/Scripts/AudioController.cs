using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {
	
	List<AudioClip> SFX = new List<AudioClip>();
	List<AudioClip> Loops = new List<AudioClip>();
	AudioSource source;
	public string InitialSong;

	private static AudioController _instance;

	public static AudioController Instance {
		get {
			if (!_instance) {
				_instance = GameObject.Find("AudioController").GetComponent<AudioController>();
			}
			return _instance;
		}
		private set {}
	}
		
	void Awake() {
		source = GetComponent<AudioSource>();
		if (AudioController.Instance && AudioController.Instance!= this) {
			//this stops the audio controller from duplicating when moving between scenes
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		foreach (object o in Resources.LoadAll("SFX")) {
			SFX.Add((AudioClip)o);
		}
		foreach (object o in Resources.LoadAll("Loops")) {
			Loops.Add((AudioClip)o);
		}
		
		CrossFadeLoop(InitialSong);
	}
	
	public void PlaySound(string soundName) {
		for (int i = 0; i < SFX.Count; i ++) {
			AudioClip wav = SFX[i];
			if (wav.name == soundName) {
				AudioSource.PlayClipAtPoint(wav, Vector3.zero, 0.4f);
				return;
			}
		}
	}

	public void PlayRandomChime () {
		int i = Random.Range (1, 4);
		PlaySound ("chime" + i.ToString ());
	}
	
	public void CrossFadeLoop(string loopName) {
		for (int i = 0; i < Loops.Count; i ++) {
			AudioClip wav = Loops[i];
			if (wav.name == loopName) {
				StartCoroutine(Cross(wav));
				return;
			}
		}
	}

	public void StopMusic () {
		GetComponent<AudioSource>().volume = 0;
	}

	IEnumerator Cross (AudioClip newLoop) {
		//If a loop is playing, fade it out
		if (source.clip) {
			while (source.volume != 0) {
				source.volume = Mathf.Max (0, source.volume - Time.deltaTime);
				yield return new WaitForEndOfFrame();
			}
		}
		//Fade in the new loop to be played
		source.clip = newLoop;
		source.volume = 0;
		source.Play();
		while (source.volume != 0.5f) {
			source.volume = Mathf.Min (0.5f, source.volume + Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
		
	}
}

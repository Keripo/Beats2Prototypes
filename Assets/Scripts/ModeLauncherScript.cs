using UnityEngine;
using System.Collections;

public class ModeLauncherScript : MonoBehaviour {
	
	// Checkboxes
	public static bool[] modesDone = new bool[8];
	
	// Music
	private const float TIME_MUSIC_DURATION	= 17f;
	private AudioSource musicSrc;
	
	private string[] modesList = {
		"Mode_1",
		"Mode_2",
		"Mode_3",
		"Mode_4",
		"Mode_5",
		"Mode_6",
		"Mode_7",
		"Mode_8"
	};
	
	// Fading
	private FadeScript fade;
	private GameObject descriptionCloseButton;
	private exSpriteFont difficultyLabel;
	private static bool descriptionViewed;
	
	// Use this for initialization
	void Start() {	
		fade = GameObject.Find("Fade").GetComponent<FadeScript>();
		fade.FadeIn();
		
		descriptionCloseButton = GameObject.Find("DescriptionCloseButton");
		difficultyLabel = GameObject.Find("DifficultyLabel").GetComponent<exSpriteFont>();
		
		difficultyLabel.text = NotesData.DIFFICULTY_LEVEL;
		
		musicSrc = (AudioSource)this.gameObject.GetComponent<AudioSource>();
		musicSrc.Play();
		
		for (int i = 0; i < ModeLauncherScript.modesDone.Length; i++) {
			string name = string.Format("Checkbox_{0}", i + 1);
			exSprite sprite = GameObject.Find(name).GetComponent<exSprite>();
			sprite.renderer.enabled = ModeLauncherScript.modesDone[i];
		}
		if (ModeLauncherScript.descriptionViewed) {
			CloseDescription();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		// Volume controlled by sin wave! With max volume of 75% and start delay of 4s
		float timeMult = ((Time.timeSinceLevelLoad - 4f) % TIME_MUSIC_DURATION) / TIME_MUSIC_DURATION;
		float volume = Mathf.Sin(Mathf.PI * 2 * timeMult) / 2 + 0.25f;
		//Debug.Log(volume);
		if (!musicSrc.isPlaying && volume >= -0.1f) {
			musicSrc.Play();
		} else if (musicSrc.isPlaying && volume <= -0.1f) {
			musicSrc.Stop();
		}
		if (volume < 0) volume = 0;
		musicSrc.volume = volume;
	
	
		// ESC / Android BACK button
		if (Input.GetKey(KeyCode.Escape)) {
			fade.FadeLaunch(null);
		}
		
		// Touch input
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				OnTapDown(touch.position);
			}
		}
		
		// Mouse click
		if (Input.GetMouseButtonDown(0)) {
			OnTapDown(Input.mousePosition);
		}
	}
	
	void CloseDescription() {
		foreach (Component component in GameObject.Find("Description").GetComponentsInChildren<Component>()) {
			if (component.gameObject.renderer != null) {
				component.gameObject.renderer.enabled = false;
			}
		}
		descriptionCloseButton.active = false;
		ModeLauncherScript.descriptionViewed = true;
	}
	
	// Tap down event
	void OnTapDown(Vector2 position) {
		// Collision check via raycast
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		// If hit
		if (Physics.Raycast (ray, out hit)) {
			// Check tag
			GameObject hitObject = hit.collider.gameObject;
			string name = hitObject.name;
			if (descriptionCloseButton != null && descriptionCloseButton.active) {
				if (name.Equals("DescriptionCloseButton")) {
					CloseDescription();
				}
			} else {
				if (name.Equals("Logo")) {
					Application.OpenURL("http://beatsportable.com");
				} else if (name.Equals("DifficultyIcon") || name.Equals("DifficultyLabel")) {
					if (NotesData.DIFFICULTY_LEVEL.Equals(NotesData.DIFFICULTY_HARD)){
						NotesData.SMOOOOCH_DATA = NotesData.SMOOOOCH_5_MOD;
						NotesData.DIFFICULTY_LEVEL = NotesData.DIFFICULTY_EASY;
					} else if (NotesData.DIFFICULTY_LEVEL.Equals(NotesData.DIFFICULTY_EASY)) {
						NotesData.SMOOOOCH_DATA = NotesData.SMOOOOCH_7;
						NotesData.DIFFICULTY_LEVEL = NotesData.DIFFICULTY_MEDIUM;
					} else if (NotesData.DIFFICULTY_LEVEL.Equals(NotesData.DIFFICULTY_MEDIUM)) {
						NotesData.SMOOOOCH_DATA = NotesData.SMOOOOCH_9;
						NotesData.DIFFICULTY_LEVEL = NotesData.DIFFICULTY_HARD;
					}
					difficultyLabel.text = NotesData.DIFFICULTY_LEVEL;
				} else {
					foreach (string mode in modesList) {
						if (name.Equals(mode)) {
							fade.FadeLaunch(mode);
							break;
						}
					}
				}
			}
		}
	}
}

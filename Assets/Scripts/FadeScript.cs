using UnityEngine;
using System.Collections;

public class FadeScript : MonoBehaviour {
	
	private const float TIME_FADE = 0.5f;
	private exSprite cover;
	private float fadeTimer;
	private int fadeType;
	private string loadLevel;
	
	void Awake () {
		cover = (exSprite)this.gameObject.GetComponent<exSprite>();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void FadeIn() {
		this.gameObject.renderer.enabled = true;
		cover.color = new Color(1, 1, 1, 1);
		fadeTimer = TIME_FADE;
		fadeType = 1; // Fade in
	}
	
	public void FadeLaunch(string loadLevel) {
		this.gameObject.renderer.enabled = true;
		cover.color = new Color(1, 1, 1, 0);
		fadeTimer = TIME_FADE;
		fadeType = 0; // Fade in
		this.loadLevel = loadLevel;
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeTimer > 0) {
			fadeTimer -= Time.deltaTime;
			if (fadeType == 1) { // Fade in
				cover.color = new Color(1, 1, 1, fadeTimer / TIME_FADE);
			} else { // Fade out
				cover.color = new Color(1, 1, 1, 1 - fadeTimer / TIME_FADE);
			}
		} else if (fadeTimer <= 0f && fadeType == 0) {
			if (loadLevel == null) {
				Application.Quit();
			} else {
				Application.LoadLevel(loadLevel);
			}
		} else {
			this.gameObject.renderer.enabled = false;
		}
	}
}

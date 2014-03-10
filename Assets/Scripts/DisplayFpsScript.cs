using UnityEngine;
using System.Collections;

public class DisplayFpsScript : MonoBehaviour {
	
	private exSpriteFont textSprite;
	private float updateDiff;
	private float timeDiff;
	private int frameDiff;
	
	// Reset counters
	void ResetCounter() {
		updateDiff = CommonScript.TIME_FPS_UPDATE;
		timeDiff = 0f;
		frameDiff = 0;		
	}
	
	// Use this for initialization
	void Start() {
		textSprite = (exSpriteFont)this.gameObject.GetComponent<exSpriteFont>();
	}
	
	// Update is called once per frame
	void Update() {
		updateDiff -= Time.deltaTime;
		timeDiff += Time.timeScale / Time.deltaTime;
		frameDiff++;
		
		if (updateDiff <= 0f) {
			float fps = timeDiff / frameDiff;
			string text = System.String.Format(
				"{0:f2} FPS",
				fps
				);
			textSprite.text = text;
			//Debug.LogWarning(text);
			ResetCounter();
		}
	}
}


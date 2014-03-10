using UnityEngine;
using System.Collections;

public class DisplayInfoScript : MonoBehaviour {

	private exSpriteFont textSprite;
	private CommonScript common;
	private int screenWidth, screenHeight;
	
	// Use this for initialization
	void Start() {		
		textSprite = (exSpriteFont)this.gameObject.GetComponent<exSpriteFont>();
		common = (CommonScript)GameObject.Find("Common").GetComponent<CommonScript>();
		screenWidth = Screen.width;
		screenHeight = Screen.height;
	}
	
	// Update is called once per frame
	void Update () {		
		string text = System.String.Format(
			"Time = {0:f3}, Display = {1}x{2}, Mode = {3}-{4}{5}",
			common.musicTime,
			screenWidth,
			screenHeight,
			common.modeNum,
			NotesData.DIFFICULTY_LEVEL,
			(common.autoPlay) ? ", AutoPlay" : ""
		);
		textSprite.text = text;
	}
}

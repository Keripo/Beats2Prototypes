using UnityEngine;
using System.Collections;

public class FeedbackScript : MonoBehaviour {
	
	private const float TIME_FADE_IN	=  3.0f;
	
	private const int COUNT_STARS		=  5;
	private const int COUNT_CATEGORY	=  5;
	private const int LAYER_STAR		= -2;
	
	private Vector3 starInitPosition	= new Vector3( 20f,  64f, LAYER_STAR);
	private Vector3 starColumnOffset	= new Vector3( 27f,   0f, 0f);
	private Vector3 starRowOffset		= new Vector3(  0f, -40f, 0f);
	
	public GameObject feedbackStarPrefab;
	
	// 2D array, 5 categories, 5 stars per
	private FeedbackStarScript[,] stars;
	private int[] ratings;
	
	// Fade timer
	private float fadeTimerStart;
	private bool fadeDone;
	
	// Submit button
	private bool submitButtonActive;
	private exSprite submitButton;
	private exSpriteFont submitLabel;
	
	// Common Resources
	private CommonScript common;
	
	// Use this for initialization
	void Start() {		
		// Disable rendering of all child items
		foreach (Component component in this.GetComponentsInChildren<Component>()) {
			if (component.gameObject.renderer != null) {
				component.gameObject.renderer.enabled = false;
			}
		}
		
		fadeTimerStart = 0;
		fadeDone = true;
		
		submitButton = GameObject.Find("SubmitButton").GetComponent<exSprite>();
		submitLabel = GameObject.Find("SubmitLabel").GetComponent<exSpriteFont>();
		submitButtonActive = false;
		
		// Disable the submit button to avoid collisions
		submitButton.gameObject.active = false;
		
		common = (CommonScript)GameObject.Find("Common").GetComponent<CommonScript>();
	}
	
	// Call this after game over
	public void Show() {
		
		// Re-enable the submit button
		submitButton.gameObject.active = true;
		
		// Re-enable rendering of all child items
		foreach (Component component in this.GetComponentsInChildren<Component>()) {
			if (component.gameObject.renderer != null) {
				component.gameObject.renderer.enabled = true;
			}
		}
		
		// Init stars
		stars = new FeedbackStarScript[COUNT_CATEGORY, COUNT_STARS];
		for (int i = 0; i < COUNT_CATEGORY; i++) {
			for (int j = 0; j < COUNT_STARS; j++) {
				GameObject starObject = (GameObject)Instantiate(feedbackStarPrefab);
				FeedbackStarScript star = starObject.GetComponent<FeedbackStarScript>();
				star.row = i; // Category
				star.column = j; // Star #
				stars[i, j] = star;
				Vector3 position = starInitPosition + i * starRowOffset + j * starColumnOffset;
				starObject.transform.position = position;
			}
		}
		ratings = new int[COUNT_CATEGORY];
		
		// Start off not appeared
		Color faded = new Color(1, 1, 1, 0);
		foreach (exSprite sprite in this.GetComponentsInChildren<exSprite>()) {
			sprite.color = faded;
		}
		foreach (exSpriteFont text in this.GetComponentsInChildren<exSpriteFont>()) {
			text.topColor = faded;
			text.botColor = faded;
		}
		foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag(Tags.FEEDBACK_STAR)) {
			gameObj.GetComponent<exSprite>().color = faded;
		}
		
		// Start fading
		fadeDone = false;
		fadeTimerStart = Time.time;
	}
	
	// Update is called once per frame
	void Update() {
		// Fade in
		if (!fadeDone) {
			float timeDiff = Time.time - fadeTimerStart;
			//Debug.LogError(timeDiff / TIME_FADE_IN);
			if (timeDiff >= TIME_FADE_IN) {
				fadeDone = true;
			}
			float alpha = timeDiff / TIME_FADE_IN;
			Color faded = new Color(1, 1, 1, alpha);
			foreach (exSprite sprite in this.GetComponentsInChildren<exSprite>()) {
				sprite.color = faded;
			}
			foreach (exSpriteFont text in this.GetComponentsInChildren<exSpriteFont>()) {
				text.topColor = faded;
				text.botColor = faded;
			}
			foreach (GameObject gameObj in GameObject.FindGameObjectsWithTag(Tags.FEEDBACK_STAR)) {
				gameObj.GetComponent<exSprite>().color = faded;
			}
			
			// Submit button is not activated until all ratings are done
			Color halfFaded = new Color(1, 1, 1, alpha * 0.3f);
			submitButton.color = halfFaded;
			submitLabel.topColor = halfFaded;
			submitLabel.botColor = halfFaded;
		}
	}
	
	// Tap down event
	public void OnTapDown(int id, Vector2 position) {
		// Collision check via raycast
		Ray ray = Camera.main.ScreenPointToRay(position);
		RaycastHit hit;
		// If hit
		if (Physics.Raycast (ray, out hit)) {
			// Check tag
			GameObject hitObject = hit.collider.gameObject;
			// TEST
			//Debug.LogError(hitObject.name);
			if (hitObject.tag == Tags.FEEDBACK_STAR) {
				FeedbackStarScript star = hitObject.GetComponent<FeedbackStarScript>();
				int row = star.row;
				int column = star.column;
				//Debug.LogError(string.Format("star ({0},{1})", row, column));
				ratings[row] = column + 1;
				for (int j = 0; j < COUNT_STARS; j++) {
					if (j <= column) {
						stars[row, j].PlaySelectAnim();
					} else {
						stars[row, j].PlayDeselectAnim();
					}
				}
				if (!submitButtonActive) {
					submitButtonActive = true;
					for (int i = 0; i < COUNT_CATEGORY; i++) {
						if (ratings[i] == 0) {
							submitButtonActive = false;
							break;
						}
					}
					if (submitButtonActive) {
						// Selectable now
						Color color = new Color(1, 1, 1, 1);
						submitButton.color = color;
						submitLabel.topColor = color;
						submitLabel.botColor = color;
					}
				}
			} else if (submitButtonActive && hitObject.name == "SubmitButton") {
				SendTrackerData();
			}
		}
	}
	
	void SendTrackerData() {
		string level = common.level;
		TrackerScript tracker = common.tracker;
		
		// Song finished
		tracker.Counter(level, "feedbackSubmitted");
		
		// Accuracy
		tracker.Average(level, "ratingChallenge", ratings[0]);
		tracker.Average(level, "ratingIntuitive", ratings[1]);
		tracker.Average(level, "ratingUnique",    ratings[2]);
		tracker.Average(level, "ratingFun",       ratings[3]);
		tracker.Average(level, "ratingOverall",   ratings[4]);
		
		// Force send
		tracker.Resume();
		
		// Exit
		common.OnBackButton();
	}
}

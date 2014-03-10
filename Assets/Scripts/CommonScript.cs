using UnityEngine;
using System.Collections;

public class CommonScript : MonoBehaviour {
	
	// Tablet vs phone
	private const float SCREEN_TABLET_PX	= 900f;  // Diagonal of a quarter of 1600x900 PC screen is big enough for "tablet"
	private const float SCREEN_TABLET_INCH	= 6f;    // Diagonal of most/all Android tablets are 7" or higher
	
	// Layer Z-values
	public const int LAYER_SLIDER			=  3;
	public const int LAYER_NOTES			=  4;
	public const int LAYER_TAPBOX			=  5;
	
	// Time constants
	public const float TIME_FPS_UPDATE		=  0.5f; // Delay between FPS counter updates
	public const float TIME_MUSIC_DELAY		= -5.0f; // Delay before music starts
	public const float TIME_LOOKAHEAD		=  0.8f; // Duration before loading note
	public const float TIME_ONSCREEN		=  0.6f; // Duration on-screen
	public const float TIME_ONSCREEN_OPAQ	=  0.5f;
	public const float TIME_ACTIVE			=  0.3f; // Duration before time before active
	public const float TIME_MISS			= -0.4f; // Duration after time before miss
	public const float TIME_REMOVE			= -0.1f; // Destroy object after this time
	public const float TIME_TEXT_FADE		=  1.0f; // Duration for combo and accuracy text
	public const float TIME_SCROLL			=  1.355f; // Duration for scroll bar, chosen experimentally
													   // so positions don't change for smooooch (~177 BPM)
	
	// Accuracy cutoffs
	public const float ACC_PRE_MARVEVLOUS	= TIME_ACTIVE * 0.1f;
	public const float ACC_PRE_PERFECT		= TIME_ACTIVE * 0.3f;
	public const float ACC_PRE_GREAT		= TIME_ACTIVE * 0.5f;
	public const float ACC_PRE_GOOD			= TIME_ACTIVE * 0.7f;
	public const float ACC_PRE_ALMOST		= TIME_ACTIVE;
	public const float ACC_POST_MARVELOUS	= TIME_MISS * 0.1f;
	public const float ACC_POST_PERFECT		= TIME_MISS * 0.3f;
	public const float ACC_POST_GREAT		= TIME_MISS * 0.5f;
	public const float ACC_POST_GOOD		= TIME_MISS * 0.7f;
	public const float ACC_POST_ALMOST		= TIME_MISS;
	public enum Accuracy {
		MARVELOUS,
		PERFECT,
		GREAT,
		GOOD,
		ALMOST,
		MISS
	};
	
	// Current game mode
	public int modeNum;
	public bool autoPlay = false;
	
	// Music and time
	public float musicTime; // Assume the delay between Update() is small
	private AudioSource musicSrc;
	private bool musicStarted;
	
	// Score
	private int comboCurrent;
	private int comboMax;
	private exSpriteFont comboText;
	private float comboTextFadeTimer;
	public float comboTextY;
	
	private int accuracyTimeDiff;
	private int[] accuracyTable;
	private Accuracy accuracyCurrent;
	private exSpriteFont accuracyText;
	private float accuracyTextFadeTimer;
	public float accuracyTextY;
	
	private int noteCount;
	private int timeDiffAvg;
	private int timeDiffAvgNoteCount;
	private float timeDiffTotal;
	private float scorePercent;
	private exSpriteFont scoreText;
	private exSpriteFont gameStateText;
	
	// Game Over
	public bool gameOver;
	private float gameOverFadeTimer;
	
	// Tracker
	public TrackerScript tracker;
	public string level;
	public float diagonalInPixels;
	public float diagonalInInches;
	
	// Fading
	private FadeScript fade;
	private FeedbackScript feedback;
	
	// Use this for initialization
	void Start () {
		SetupTracker();
		SetupFade();
		SetupTime();
		SetupScore();
	}
	
	void SetupTracker() {
		tracker = GameObject.Find("Tracker").GetComponent<TrackerScript>();
		
		// Determine if tablet or phone
		string screenType = "?";
		if (Application.platform == RuntimePlatform.Android) {
			float widthInchesSqrd = Mathf.Pow(DisplayMetricsAndroid.WidthPixels / DisplayMetricsAndroid.XDPI, 2);
			float heightInchesSqrd = Mathf.Pow(DisplayMetricsAndroid.HeightPixels / DisplayMetricsAndroid.YDPI, 2);
			diagonalInInches = Mathf.Sqrt(widthInchesSqrd + heightInchesSqrd);
			//Debug.Log(string.Format("diagonalInInches = {0}", diagonalInInches));
			if (diagonalInInches < SCREEN_TABLET_INCH) {
				screenType = "P";
			} else {
				screenType = "T";
			}
		} else {
			// Lets just use pixels
			diagonalInPixels = Mathf.Sqrt(Mathf.Pow(Screen.width, 2) + Mathf.Pow(Screen.height, 2));
			//Debug.Log(string.Format("diagonalInPixels = {0}", diagonalInPixels));
			if (diagonalInPixels < SCREEN_TABLET_PX) {
				screenType = "P";
			} else {
				screenType = "T";
			}
		}
		
		// Level name based on mode number
		level = string.Format("{0}{1}-{2}", modeNum, screenType, NotesData.DIFFICULTY_LEVEL.Substring(0, 1));
		
		// Song started
		tracker.Counter(level, "started");
		
		// Force send
		tracker.Resume();
		tracker.Pause();
	}
	
	// Fade in
	void SetupFade() {
		fade = GameObject.Find("Fade").GetComponent<FadeScript>();
		fade.FadeIn();
		feedback = GameObject.Find("Feedback").GetComponent<FeedbackScript>();
	}
	
	// Setup music and global time
	void SetupTime() {
		musicTime = TIME_MUSIC_DELAY;
		musicSrc = (AudioSource)this.gameObject.GetComponent<AudioSource>();
	}
	
	void SetupScore() {
		// Score
		comboCurrent = 0;
		comboMax = 0;
		accuracyTable = new int[System.Enum.GetNames(typeof(Accuracy)).Length + 1];
		comboText = (exSpriteFont)GameObject.Find("ComboText").GetComponent<exSpriteFont>();
		comboTextFadeTimer = 0f;
		comboText.text = "";
		comboText.gameObject.transform.position = new Vector3(0, comboTextY, 0);
		accuracyText = (exSpriteFont)GameObject.Find("AccuracyText").GetComponent<exSpriteFont>();
		accuracyTextFadeTimer = 0f;
		accuracyText.text = "";
		accuracyText.gameObject.transform.position = new Vector3(0, accuracyTextY, 0);
		accuracyTimeDiff = 0;
		scoreText = (exSpriteFont)GameObject.Find("ScoreText").GetComponent<exSpriteFont>();
		scorePercent = 0f;
		
		// Game state
		gameStateText = (exSpriteFont)GameObject.Find("GameStateText").GetComponent<exSpriteFont>();
		gameStateText.text = "Ready";
		gameOver = false;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTime();
		UpdateScore();
	}
	
	// Update global time
	void UpdateTime() {
		if (!musicStarted) {
			musicTime = Time.timeSinceLevelLoad + TIME_MUSIC_DELAY;
			if (musicTime > TIME_MUSIC_DELAY / 2) {
				gameStateText.topColor = new Color(1, 1, 1, musicTime / (TIME_MUSIC_DELAY / 2));
				gameStateText.botColor = gameStateText.topColor;
				System.GC.Collect(); // Try to force cleanups
			}
			
			// Play music once delay over
			if (musicTime > 0) {
				musicStarted = true;
				musicSrc.Play();
				gameStateText.text = "";
			}
		} else {
			musicTime = musicSrc.time;
		}
	}
	
	// Update score text
	void UpdateScore() {
		if (gameOver) {
			gameOverFadeTimer -= Time.deltaTime;
			if (gameOverFadeTimer > 0 && gameOverFadeTimer > -TIME_MUSIC_DELAY / 2) {
				gameStateText.topColor =
				gameStateText.botColor =
				gameStateText.topColor =
				new Color(1, 1, 1, 1 - (gameOverFadeTimer - (-TIME_MUSIC_DELAY / 2)));
			} else {
				gameStateText.topColor =
				gameStateText.botColor =
				gameStateText.topColor =
				Color.white;
			}
		}		
		if (comboTextFadeTimer > 0 && comboCurrent > 0) {
			comboText.text = string.Format("{0} Combo", comboCurrent);
			// Fade out
			comboTextFadeTimer -= Time.deltaTime;
			if (comboTextFadeTimer < TIME_TEXT_FADE / 2) {
				comboText.topColor = new Color(1, 1, 1, comboTextFadeTimer / (TIME_TEXT_FADE / 2));
				comboText.botColor = comboText.topColor;
			}
		}
		if (accuracyTextFadeTimer > 0) {
			//accuracyText.text = string.Format("{0} ({1}ms)", accuracyCurrent.ToString(), accuracyTimeDiff);
			accuracyText.text = string.Format("{0}", accuracyCurrent.ToString());
			// Fade out
			accuracyTextFadeTimer -= Time.deltaTime;
			if (accuracyTextFadeTimer < TIME_TEXT_FADE / 2) {
				accuracyText.topColor = new Color(1, 1, 1, accuracyTextFadeTimer / (TIME_TEXT_FADE / 2));
				accuracyText.botColor = accuracyText.topColor;
			}
		}
		scoreText.text = string.Format(
			"Percent: {0:f1}%\n" +
			"{1} MARVELOUS\n" +
			"{2} PERFECT\n" +
			"{3} GREAT\n" +
			"{4} GOOD\n" + 
			"{5} ALMOST\n" +
			"{6} MISS\n" +
			"{7} Combo MAX",
			scorePercent,
			accuracyTable.GetValue((int)Accuracy.MARVELOUS),
			accuracyTable.GetValue((int)Accuracy.PERFECT),
			accuracyTable.GetValue((int)Accuracy.GREAT),
			accuracyTable.GetValue((int)Accuracy.GOOD),
			accuracyTable.GetValue((int)Accuracy.ALMOST),
			accuracyTable.GetValue((int)Accuracy.MISS),
			comboMax
		);
	}
	
	// Check accuracy vs values
	public void SetAccuracy(float timeDiff) {
		if (timeDiff >= 0) {
			if (timeDiff < ACC_PRE_MARVEVLOUS) {
				accuracyCurrent = Accuracy.MARVELOUS;
			} else if (timeDiff < ACC_PRE_PERFECT) {
				accuracyCurrent = Accuracy.PERFECT;
			} else if (timeDiff < ACC_PRE_GREAT) {
				accuracyCurrent = Accuracy.GREAT;
			} else if (timeDiff < ACC_PRE_GOOD) {
				accuracyCurrent = Accuracy.GOOD;
			} else if (timeDiff < ACC_PRE_ALMOST) {
				accuracyCurrent = Accuracy.ALMOST;
			} else {
				accuracyCurrent = Accuracy.MISS;
			}
		} else {
			if (timeDiff > ACC_POST_MARVELOUS) {
				accuracyCurrent = Accuracy.MARVELOUS;
			} else if (timeDiff > ACC_POST_PERFECT) {
				accuracyCurrent = Accuracy.PERFECT;
			} else if (timeDiff > ACC_POST_GREAT) {
				accuracyCurrent = Accuracy.GREAT;
			} else if (timeDiff > ACC_POST_GOOD) {
				accuracyCurrent = Accuracy.GOOD;
			} else if (timeDiff > ACC_POST_ALMOST) {
				accuracyCurrent = Accuracy.ALMOST;
			} else {
				accuracyCurrent = Accuracy.MISS;
			}
		}
		switch (accuracyCurrent) {
			case Accuracy.MARVELOUS:
			case Accuracy.PERFECT:
			case Accuracy.GREAT:
				// Increase combo
				comboCurrent++;
				if (comboCurrent > comboMax) {
					comboMax = comboCurrent;
				}
				comboTextFadeTimer = TIME_TEXT_FADE;
				comboText.topColor = new Color(1, 1, 1, 1);
				comboText.botColor = comboText.topColor;
				break;
			default:
				// Reset
				comboCurrent = 0;
				comboTextFadeTimer = 0f;
				comboText.text = "";
				break;
		}
		accuracyTable[(int)accuracyCurrent]++;
		accuracyTextFadeTimer = TIME_TEXT_FADE;
		accuracyText.topColor = new Color(1, 1, 1, 1);
		accuracyText.botColor = accuracyText.topColor;
		accuracyTimeDiff = (int)(timeDiff * 1000);
		
		noteCount++;
		if (comboCurrent >= 1) { // Only average good hits
			timeDiffAvg = (timeDiffAvg * timeDiffAvgNoteCount + accuracyTimeDiff) / (timeDiffAvgNoteCount + 1);
			timeDiffAvgNoteCount++;
		}
		if (timeDiff >= 0) {
			timeDiffTotal += timeDiff;
		} else {
			timeDiffTotal -= timeDiff;
		}
		scorePercent = (1 - (timeDiffTotal / (noteCount * (-TIME_MISS)))) * 100f;
		if (scorePercent < 0) {
			scorePercent = 0f;
		}
	}
	
	// Tap down event
	public void OnTapDown(int id, Vector2 position) {
		if (gameOver) {
			feedback.OnTapDown(id, position);
		}
	}
	
	public void OnBackButton() {
		fade.FadeLaunch("Launcher");
	}
	
	// Note hit
	public void OnNoteHit(NotesScript note){
		float timeDiff = note.time - musicTime;
		SetAccuracy(timeDiff);
		note.state = NotesScript.NotesState.HIT;
		note.PlayHitAnim();
		note.time_hit = musicTime;
	}
	
	public void OnNoteMiss(NotesScript note) {
		float timeDiff = note.time - musicTime;
		SetAccuracy(timeDiff);
		note.state = NotesScript.NotesState.MISS;
		note.PlayMissAnim();
	}
	
	public void OnGameOver() {
		gameStateText.text = "Complete!";
		gameOver = true;
		gameOverFadeTimer = -TIME_MUSIC_DELAY;
		ModeLauncherScript.modesDone[this.modeNum - 1] = true;
		SendTrackerData();
		feedback.Show();
	}
	
	public void SendTrackerData() {
		// Song finished
		tracker.Counter(level, "completed");
		
		// Accuracy
		tracker.Average(level, "scorePercent",    Mathf.RoundToInt(scorePercent)); // Rounded is good enough
		tracker.Average(level, "timeDiffAvg",     timeDiffAvg);
		tracker.Average(level, "hit_5_MARVELOUS", (int)accuracyTable.GetValue((int)Accuracy.MARVELOUS));
		tracker.Average(level, "hit_4_PERFECT",   (int)accuracyTable.GetValue((int)Accuracy.PERFECT));
		tracker.Average(level, "hit_3_GREAT",     (int)accuracyTable.GetValue((int)Accuracy.GREAT));
		tracker.Average(level, "hit_2_GOOD",      (int)accuracyTable.GetValue((int)Accuracy.GOOD));
		tracker.Average(level, "hit_1_ALMOST",    (int)accuracyTable.GetValue((int)Accuracy.ALMOST));
		tracker.Average(level, "hit_0_MISS",      (int)accuracyTable.GetValue((int)Accuracy.MISS));
		
		// Max combos
		tracker.Average(level, "comboMax", comboMax);
		
		// Force send
		tracker.Resume();
	}
	
	public float GetTimeDiff(NotesScript note) {
		return note.time - musicTime;
	}
	
	public void CheckAutoPlay(NotesScript note, float timeDiff) {
		if (autoPlay && note.state == NotesScript.NotesState.ACTIVE) {
			if (timeDiff < ACC_PRE_MARVEVLOUS / 2) { // Super accuracy!
				OnNoteHit(note);
			}
		}
	}
	
	public void UpdateNoteState(NotesScript note, float timeDiff) {
		// Update alpha
		exSprite sprite = note.gameObject.GetComponent<exSprite>();
		if (timeDiff > TIME_ONSCREEN_OPAQ) {
			sprite.color = new Color(1, 1, 1, 1 - (timeDiff - TIME_ONSCREEN_OPAQ) / (TIME_LOOKAHEAD - TIME_ONSCREEN_OPAQ));
		} else {
			sprite.color = new Color(1, 1, 1, 1);
		}
		
		// We assume notes don't skip states (e.g. from DISABLE to MISS)
		switch(note.state) {
			case NotesScript.NotesState.DISABLE:
				if (timeDiff <= TIME_ACTIVE) {
					note.state = NotesScript.NotesState.ACTIVE;
				}
				break;
			case NotesScript.NotesState.ACTIVE:
				if (timeDiff <= TIME_MISS) {
					OnNoteMiss(note);
				}
				break;
			case NotesScript.NotesState.HIT:
				float hitTimeDiff = note.time_hit - musicTime;
				if (hitTimeDiff <= TIME_REMOVE) {
					note.state = NotesScript.NotesState.REMOVE;
				}
				break;
			case NotesScript.NotesState.MISS:
				// TODO
				if (timeDiff <= TIME_MISS + TIME_REMOVE) {
					note.state = NotesScript.NotesState.REMOVE;
				}
				break;
			case NotesScript.NotesState.REMOVE:
			default:
				break;
		}
	}
}

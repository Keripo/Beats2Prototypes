using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeGridScript : MonoBehaviour {
	
	// Positions
	private Vector3 tapboxPosition_0		= new Vector3(-120f,  120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_1		= new Vector3( -40f,  120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_2		= new Vector3(  40f,  120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_3		= new Vector3( 120f,  120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_4		= new Vector3(-120f,   40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_5		= new Vector3( -40f,   40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_6		= new Vector3(  40f,   40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_7		= new Vector3( 120f,   40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_8		= new Vector3(-120f,  -40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_9		= new Vector3( -40f,  -40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_10		= new Vector3(  40f,  -40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_11		= new Vector3( 120f,  -40f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_12		= new Vector3(-120f, -120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_13		= new Vector3( -40f, -120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_14		= new Vector3(  40f, -120f, LAYER_TAPBOX);
	private Vector3 tapboxPosition_15		= new Vector3( 120f, -120f, LAYER_TAPBOX);
	private Vector3 notesPositionOffset		= new Vector3(    0,     0,          -1f);
	private Vector2 gridScale				= new Vector2(1.4f, 1.4f);
	
	// Layer constants
	private const int LAYER_BG				= 10;
	private const int LAYER_TAPBOX			=  5;
	private const int LAYER_NOTES			=  4;
	private const int LAYER_INTERFACE		=  0;
	
	// Common Resources
	private CommonScript common;
		
	// Tapboxes
	public GameObject tapboxPrefab;
	private Dictionary<int, TapboxScript> touchMap;
	private List<TapboxScript> tapboxes;
	
	// Notes
	public GameObject notesPrefab;
	private NotesIterator notesIterator;
	private LinkedList<NotesScript> notes; // TODO - Assume sorted
	
	// Awake is called prior to Start()
	void Awake() {
		this.gameObject.tag = Tags.MAIN;
	}
	
	// Use this for initialization
	void Start() {
		SetupCommon();
		SetupInput();
		SetupTapboxes();
		SetupNotes();
	}
	
	// Setup common and game state
	void SetupCommon() {
		common = (CommonScript)GameObject.Find("Common").GetComponent<CommonScript>();
	}
	
	// Setup touchmap
	void SetupInput() {
		touchMap = new Dictionary<int, TapboxScript>();
	}
	
	// Setup tapboxes and touch input
	void SetupTapboxes() {
		tapboxes = new List<TapboxScript>();
		tapboxes.Add(InitTapbox(0, tapboxPosition_0));
		tapboxes.Add(InitTapbox(1, tapboxPosition_1));
		tapboxes.Add(InitTapbox(2, tapboxPosition_2));
		tapboxes.Add(InitTapbox(3, tapboxPosition_3));
		tapboxes.Add(InitTapbox(4, tapboxPosition_4));
		tapboxes.Add(InitTapbox(5, tapboxPosition_5));
		tapboxes.Add(InitTapbox(6, tapboxPosition_6));
		tapboxes.Add(InitTapbox(7, tapboxPosition_7));
		tapboxes.Add(InitTapbox(8, tapboxPosition_8));
		tapboxes.Add(InitTapbox(9, tapboxPosition_9));
		tapboxes.Add(InitTapbox(10, tapboxPosition_10));
		tapboxes.Add(InitTapbox(11, tapboxPosition_11));
		tapboxes.Add(InitTapbox(12, tapboxPosition_12));
		tapboxes.Add(InitTapbox(13, tapboxPosition_13));
		tapboxes.Add(InitTapbox(14, tapboxPosition_14));
		tapboxes.Add(InitTapbox(15, tapboxPosition_15));
	}
	
	// Setup notes data
	void SetupNotes() {
		notesIterator = new NotesIterator(NotesData.SMOOOOCH_DATA); // TODO - hardcoded
		notes = new LinkedList<NotesScript>();
	}
	
	// Initialize a tapbox
	TapboxScript InitTapbox(int column, Vector3 position) {
		GameObject tapboxObject = (GameObject)Instantiate(tapboxPrefab);
		TapboxScript tapbox = tapboxObject.GetComponent<TapboxScript>();
		tapbox.Setup(column);
		tapboxObject.transform.position = position;
		exSprite sprite = tapboxObject.GetComponent<exSprite>();
		sprite.scale = gridScale;
		return tapbox;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateInput();
		UpdateNotesList();
		UpdateNotes();
	}
	
	// Check for keyboard, touch, and mouse input
	void UpdateInput() {
		// ESC / Android BACK button
		if (Input.GetKey(KeyCode.Escape)) {
			common.OnBackButton();
		}
		
		// Touch input
		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				OnTapDown(touch.fingerId, touch.position);
			} else if (touch.phase == TouchPhase.Ended) {
				OnTapUp(touch.fingerId);
			}
		}
		
		// Mouse click
		if (Input.GetMouseButtonDown(0)) {
			OnTapDown(0, Input.mousePosition);
		} else if (Input.GetMouseButtonUp(0)) {
			OnTapUp(0);
		}
	}
	
	// Tap down event
	void OnTapDown(int id, Vector2 position) {
		if (!touchMap.ContainsKey(id)) { // Protect against double counts
			if (common.gameOver) {
				common.OnTapDown(id, position);
			} else {
				// Collision check via raycast
				Ray ray = Camera.main.ScreenPointToRay(position);
				RaycastHit hit;
				// If hit
				if (Physics.Raycast (ray, out hit)) {
					// Check tag
					GameObject hitObject = hit.collider.gameObject;
					if (hitObject.tag.Equals(Tags.TAPBOX)) {
						TapboxScript tapbox = hitObject.GetComponent<TapboxScript>();
						// Animation
						tapbox.PlayDownAnim();
						// Add to dictionary
						touchMap.Add(
							id,
							tapbox
						);
						// Check for notes
						OnTapboxTap(tapbox);
					}
				}
			}
		}
	}
	
	// Tap up event
	void OnTapUp(int id) {
		if (touchMap.ContainsKey(id)) { // Protect against double counts
			// Check dictionary
			TapboxScript tapbox;
			if (touchMap.TryGetValue(id, out tapbox)) {
				// Animation
				tapbox.PlayUpAnim();
				// Remove from dictionary
				touchMap.Remove(id);
				// TODO - add lift note action
			}
		}
	}
	
	// Check for notes hit
	void OnTapboxTap(TapboxScript tapbox) {
		int column = tapbox.column;
		foreach (NotesScript note in notes) {
			if (note.column == column) {
				if (note.state == NotesScript.NotesState.ACTIVE) {
					common.OnNoteHit(note);
					break;
				}
			}
		}
	}
	
	// Remove completed notes, add new ones
	private int rowCount = 0;
	void UpdateNotesList() {
		
		// Game over check
		if (common.gameOver) return;
		
		// Remove completed notes, assumes sequential removal
		while(notes.Count > 0 && notes.First.Value.state == NotesScript.NotesState.REMOVE) {
			Destroy(notes.First.Value.gameObject);
			notes.RemoveFirst();
		}
		
		// Add new notes
		while (notesIterator.hasNext()) {
			// If in the look-ahead range
			if (notesIterator.nextTime() - common.musicTime < CommonScript.TIME_LOOKAHEAD) {
				GameObject notesObject = (GameObject)Instantiate(notesPrefab);
				NotesScript note = notesObject.GetComponent<NotesScript>();
				notesIterator.next(note);
				// For grid
				note.column += rowCount * 4;
				rowCount++;
				if (rowCount > 3) rowCount = 0;
				Vector3 position;
				switch(note.column) {
					case 0: position = tapboxPosition_0; break;
					case 1: position = tapboxPosition_1; break;
					case 2: position = tapboxPosition_2; break;
					case 3: position = tapboxPosition_3; break;
					case 4: position = tapboxPosition_4; break;
					case 5: position = tapboxPosition_5; break;
					case 6: position = tapboxPosition_6; break;
					case 7: position = tapboxPosition_7; break;
					case 8: position = tapboxPosition_8; break;
					case 9: position = tapboxPosition_9; break;
					case 10: position = tapboxPosition_10; break;
					case 11: position = tapboxPosition_11; break;
					case 12: position = tapboxPosition_12; break;
					case 13: position = tapboxPosition_13; break;
					case 14: position = tapboxPosition_14; break;
					case 15: position = tapboxPosition_15; break;
					default: position = new Vector3(0, 0, 0); break; // Error
				}
				position += notesPositionOffset;
				note.gameObject.transform.position = position;
				// Add
				notes.AddLast(new LinkedListNode<NotesScript>(note));
			} else {
				break;
			}
		}
		
		// Check game done
		if (notes.Count == 0 && !notesIterator.hasNext()) {
			foreach (TapboxScript tapbox in tapboxes) {
				tapbox.gameObject.active = false;
			}
			common.OnGameOver();
		}
	}
	
	// Update notes states
	void UpdateNotes() {
	
		// Game over check
		if (common.gameOver) return;
		
		// Iterate through each active node
		foreach (NotesScript note in notes) {
			float timeDiff = common.GetTimeDiff(note);
			common.CheckAutoPlay(note, timeDiff);
			common.UpdateNoteState(note, timeDiff);
			
			// Update size
			exSprite sprite = note.gameObject.GetComponent<exSprite>();
			if (timeDiff > 0) { // Before tapbox
				float multiplier = 1 - timeDiff / CommonScript.TIME_ONSCREEN;
				if (multiplier <= 0f) {
					// Don't draw
					note.gameObject.active = false;
				} else {
					sprite.scale = gridScale * multiplier;
					// Draw
					note.gameObject.active = true;
				}
			} else {
				sprite.scale = gridScale;
				note.gameObject.active = true;
			}
		}
	}
}

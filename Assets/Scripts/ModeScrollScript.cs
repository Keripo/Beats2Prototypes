using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeScrollScript : MonoBehaviour {
	
	// For Mode 2, to prevent appearing prior to centre
	public bool restrainEarly = false;
	
	// Positions
	public Vector3 tapboxPosition_0		= new Vector3( 120f, -120f, CommonScript.LAYER_TAPBOX);
	public Vector3 tapboxPosition_1		= new Vector3(  40f, -120f, CommonScript.LAYER_TAPBOX);
	public Vector3 tapboxPosition_2		= new Vector3( -40f, -120f, CommonScript.LAYER_TAPBOX);
	public Vector3 tapboxPosition_3		= new Vector3(-120f, -120f, CommonScript.LAYER_TAPBOX);
	
	public Vector3 notesPositionInit_0	= new Vector3( 120f,  400f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_1	= new Vector3(  40f,  400f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_2	= new Vector3( -40f,  400f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_3	= new Vector3(-120f,  400f, CommonScript.LAYER_NOTES);
	
	public Vector3 notesPositionEnd_0	= new Vector3( 120f, -120f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_1	= new Vector3(  40f, -120f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_2	= new Vector3( -40f, -120f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_3	= new Vector3(-120f, -120f, CommonScript.LAYER_NOTES);
	
	private Vector3 notesPositionDelta_0, notesPositionDelta_1, notesPositionDelta_2, notesPositionDelta_3;
	
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
		for (int i = 0; i < 9; i++) {
			touchMap.Add(i, null);
		}
		for (int i = 0; i < 9; i++) {
			touchMap.Remove(i);
		}
	}
	
	// Setup tapboxes and touch input
	void SetupTapboxes() {
		tapboxes = new List<TapboxScript>();
		tapboxes.Add(InitTapbox(0, tapboxPosition_0));
		tapboxes.Add(InitTapbox(1, tapboxPosition_1));
		tapboxes.Add(InitTapbox(2, tapboxPosition_2));
		tapboxes.Add(InitTapbox(3, tapboxPosition_3));
	}
	
	// Setup notes data
	void SetupNotes() {
		notesIterator = new NotesIterator(NotesData.SMOOOOCH_DATA); // TODO - hardcoded
		notes = new LinkedList<NotesScript>();
		notesPositionDelta_0 = notesPositionInit_0 - notesPositionEnd_0;
		notesPositionDelta_1 = notesPositionInit_1 - notesPositionEnd_1;
		notesPositionDelta_2 = notesPositionInit_2 - notesPositionEnd_2;
		notesPositionDelta_3 = notesPositionInit_3 - notesPositionEnd_3;
	}
	
	// Initialize a tapbox
	TapboxScript InitTapbox(int column, Vector3 position) {
		GameObject tapboxObject = (GameObject)Instantiate(tapboxPrefab);
		TapboxScript tapbox = tapboxObject.GetComponent<TapboxScript>();
		tapbox.Setup(column);
		tapboxObject.transform.position = position;
		return tapbox;
	}
	
	// Update is called once per frame
	void Update () {
		// Note: parse input before updating the time and notes
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
			//if (true) {
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
			
			// Update position
			Vector3 position;
			switch(note.column) {
				case 0: position = notesPositionEnd_0; break;
				case 1: position = notesPositionEnd_1; break;
				case 2: position = notesPositionEnd_2; break;
				case 3: position = notesPositionEnd_3; break;
				default: position = new Vector3(0, 0, 0); break; // Error
			}
			if (timeDiff > 0) { // Before tapbox
				float multiplier = timeDiff / CommonScript.TIME_ONSCREEN;
				if (restrainEarly && multiplier > 1f) {
					// Don't draw
					note.gameObject.active = false;
				} else {
					// Shift position by offset
					Vector3 offset;
					switch(note.column) {
						case 0: offset = notesPositionDelta_0 * multiplier; break;
						case 1: offset = notesPositionDelta_1 * multiplier; break;
						case 2: offset = notesPositionDelta_2 * multiplier; break;
						case 3: offset = notesPositionDelta_3 * multiplier; break;
						default: offset = new Vector3(0, 0, 0); break; // Error
					}
					position += offset;
					// Draw
					note.gameObject.active = true;
				}
			}
			note.gameObject.transform.position = position;
		}
	}
}

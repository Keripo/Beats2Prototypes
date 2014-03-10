using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeAppearScript : MonoBehaviour {
	
	// Positions
	private Vector3 notesPosition_0			= new Vector3(-120f,  120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_1			= new Vector3( -40f,  120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_2			= new Vector3(  40f,  120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_3			= new Vector3( 120f,  120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_4			= new Vector3(-120f,   40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_5			= new Vector3( -40f,   40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_6			= new Vector3(  40f,   40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_7			= new Vector3( 120f,   40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_8			= new Vector3(-120f,  -40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_9			= new Vector3( -40f,  -40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_10		= new Vector3(  40f,  -40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_11		= new Vector3( 120f,  -40f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_12		= new Vector3(-120f, -120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_13		= new Vector3( -40f, -120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_14		= new Vector3(  40f, -120f, CommonScript.LAYER_NOTES);
	private Vector3 notesPosition_15		= new Vector3( 120f, -120f, CommonScript.LAYER_NOTES);
	
	private Vector2 tapboxSize				= new Vector2 (1.22f,  1.22f);
	
	// Common Resources
	private CommonScript common;
		
	// Tapboxes
	public GameObject tapboxPrefab;
	
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
	}
	
	// Setup tapboxes and touch input
	void SetupTapboxes() {
	}
	
	// Setup notes data
	void SetupNotes() {
		notesIterator = new NotesIterator(NotesData.SMOOOOCH_DATA); // TODO - hardcoded
		notes = new LinkedList<NotesScript>();
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
			}
		}
		
		// Mouse click
		if (Input.GetMouseButtonDown(0)) {
			OnTapDown(0, Input.mousePosition);;
		}
	}
	
	// Tap down event
	void OnTapDown(int id, Vector2 position) {
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
				if (hitObject.tag.Equals(Tags.NOTE)) {
					NotesScript note = hitObject.GetComponent<NotesScript>();
					if (note.state == NotesScript.NotesState.ACTIVE) {
						common.OnNoteHit(note);
					}
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
			// Destroy the tapbox
			Destroy(notes.First.Value.tapbox.gameObject);
			// Destroy the note
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
					case 0: position = notesPosition_0; break;
					case 1: position = notesPosition_1; break;
					case 2: position = notesPosition_2; break;
					case 3: position = notesPosition_3; break;
					case 4: position = notesPosition_4; break;
					case 5: position = notesPosition_5; break;
					case 6: position = notesPosition_6; break;
					case 7: position = notesPosition_7; break;
					case 8: position = notesPosition_8; break;
					case 9: position = notesPosition_9; break;
					case 10: position = notesPosition_10; break;
					case 11: position = notesPosition_11; break;
					case 12: position = notesPosition_12; break;
					case 13: position = notesPosition_13; break;
					case 14: position = notesPosition_14; break;
					case 15: position = notesPosition_15; break;
					default: position = new Vector3(0, 0, 0); break; // Error
				}
				note.gameObject.transform.position = position;
				// Add
				notes.AddLast(new LinkedListNode<NotesScript>(note));
				
				// Add corresponding tapbox
				GameObject tapboxObject = (GameObject)Instantiate(tapboxPrefab);
				TapboxScript tapbox = tapboxObject.GetComponent<TapboxScript>();
				tapboxObject.transform.position = position;
				note.tapbox = tapbox;
			} else {
				break;
			}
		}
		
		// Check game done
		if (notes.Count == 0 && !notesIterator.hasNext()) {
			common.OnGameOver();;
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
			
			// Update tapbox size
			exSprite sprite = note.tapbox.gameObject.GetComponent<exSprite>();
			if (timeDiff > 0) { // Before tapbox
				float multiplier = timeDiff / CommonScript.TIME_ONSCREEN;
				sprite.scale = tapboxSize * (1 + multiplier);
			} else {
				sprite.scale = tapboxSize;
			}
		}
	}
}

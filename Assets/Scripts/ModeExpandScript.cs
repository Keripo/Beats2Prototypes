using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeExpandScript : MonoBehaviour {
	
	// Positions
	public Vector3 sliderPositionInit_0		= new Vector3(   0f,    0f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionInit_1		= new Vector3(   0f,    0f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionInit_2		= new Vector3(   0f,    0f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionInit_3		= new Vector3(   0f,    0f, CommonScript.LAYER_SLIDER);
	
	public Vector3 sliderPositionEnd_0		= new Vector3(   0f, -150f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionEnd_1		= new Vector3(   0f,  150f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionEnd_2		= new Vector3(-150f,    0f, CommonScript.LAYER_SLIDER);
	public Vector3 sliderPositionEnd_3		= new Vector3( 150f,    0f, CommonScript.LAYER_SLIDER);
	
	public Vector3 notesPositionInit_0		= new Vector3(   0f,    0f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_1		= new Vector3(   0f,    0f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_2		= new Vector3(   0f,    0f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionInit_3		= new Vector3(   0f,    0f, CommonScript.LAYER_NOTES);
	
	public Vector3 notesPositionEnd_0		= new Vector3(-150f, -150f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_1		= new Vector3(-150f,  150f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_2		= new Vector3( 150f,  150f, CommonScript.LAYER_NOTES);
	public Vector3 notesPositionEnd_3		= new Vector3( 150f, -150f, CommonScript.LAYER_NOTES);
	
	private Vector3 notesPositionDelta_0, notesPositionDelta_1, notesPositionDelta_2, notesPositionDelta_3;
	private Vector3 sliderPositionDelta_0, sliderPositionDelta_1, sliderPositionDelta_2, sliderPositionDelta_3;
	
	public Vector2 sliderSizeInit			= new Vector2(   0f, 0.3f);
	public Vector2 sliderSizeEnd			= new Vector2(0.95f, 0.3f);
	private Vector2 sliderSizeDelta;
	
	// Common Resources
	private CommonScript common;
		
	// Tapboxes
	public GameObject tapboxPrefab;
	private TapboxScript slider_0, slider_1, slider_2, slider_3;
	
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
		//tapboxes = new List<TapboxScript>();
		GameObject sliderObject_0 = (GameObject)Instantiate(tapboxPrefab);
		slider_0 = sliderObject_0.GetComponent<TapboxScript>();
		exSprite sprite_0 = sliderObject_0.GetComponent<exSprite>();
		sprite_0.scale = sliderSizeInit;
		sprite_0.color = new Color(1, 1, 1, 0.75f);
		sliderPositionDelta_0 = sliderPositionInit_0 - sliderPositionEnd_0;
		
		GameObject sliderObject_1 = (GameObject)Instantiate(tapboxPrefab);
		slider_1 = sliderObject_1.GetComponent<TapboxScript>();
		exSprite sprite_1 = sliderObject_1.GetComponent<exSprite>();
		sprite_1.scale = sliderSizeInit;
		sprite_1.color = new Color(1, 1, 1, 0.75f);
		sliderPositionDelta_1 = sliderPositionInit_1 - sliderPositionEnd_1;
		
		GameObject sliderObject_2 = (GameObject)Instantiate(tapboxPrefab);
		slider_2 = sliderObject_2.GetComponent<TapboxScript>();
		exSprite sprite_2 = sliderObject_2.GetComponent<exSprite>();
		sprite_2.scale = sliderSizeInit;
		sprite_2.color = new Color(1, 1, 1, 0.75f);
		sliderObject_2.transform.Rotate(new Vector3(0, 0, 90f));
		sliderPositionDelta_2 = sliderPositionInit_2 - sliderPositionEnd_2;
		
		GameObject sliderObject_3 = (GameObject)Instantiate(tapboxPrefab);
		slider_3 = sliderObject_3.GetComponent<TapboxScript>();
		exSprite sprite_3 = sliderObject_3.GetComponent<exSprite>();
		sprite_3.scale = sliderSizeInit;
		sprite_3.color = new Color(1, 1, 1, 0.75f);
		sliderObject_3.transform.Rotate(new Vector3(0, 0, 90f));
		sliderPositionDelta_3 = sliderPositionInit_3 - sliderPositionEnd_3;
		
		sliderSizeDelta = sliderSizeInit - sliderSizeEnd;
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
			//} else if (touch.phase == TouchPhase.Ended) {
			//	OnTapUp(touch.fingerId);
			}
		}
		
		// Mouse click
		if (Input.GetMouseButtonDown(0)) {
			OnTapDown(0, Input.mousePosition);
		//} else if (Input.GetMouseButtonUp(0)) {
		//	OnTapUp(0);
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
				// Set note's position
				Vector3 position;
				float timePoint = note.time % CommonScript.TIME_SCROLL;
				float multiplier = timePoint / CommonScript.TIME_SCROLL;
				switch(note.column) {
					case 0: position = notesPositionInit_0 - notesPositionDelta_0 * multiplier; break;
					case 1: position = notesPositionInit_1 - notesPositionDelta_1 * multiplier; break;
					case 2: position = notesPositionInit_2 - notesPositionDelta_2 * multiplier; break;
					case 3: position = notesPositionInit_3 - notesPositionDelta_3 * multiplier; break;
					default: position = new Vector3(0, 0, 0); break; // Error
				}
				notesObject.transform.position = position;
				exSprite sprite = notesObject.GetComponent<exSprite>();
				sprite.color = new Color(1, 1, 1, 0);
				notes.AddLast(new LinkedListNode<NotesScript>(note));
			} else {
				break;
			}
		}
		
		// Check game done
		if (notes.Count == 0 && !notesIterator.hasNext()) {
			slider_0.gameObject.active = false;
			slider_1.gameObject.active = false;
			slider_2.gameObject.active = false;
			slider_3.gameObject.active = false;
			common.OnGameOver();
		}
	}
	
	// Update notes states
	void UpdateNotes() {
	
		// Game over check
		if (common.gameOver) return;
		
		// Update slider position
		float multiplier = (common.musicTime % CommonScript.TIME_SCROLL) / CommonScript.TIME_SCROLL;
		if (multiplier >= 0) {
			slider_0.gameObject.transform.position = sliderPositionInit_0 - sliderPositionDelta_0 * multiplier;
			slider_0.gameObject.GetComponent<exSprite>().scale = sliderSizeInit - sliderSizeDelta * multiplier;
			
			slider_1.gameObject.transform.position = sliderPositionInit_1 - sliderPositionDelta_1 * multiplier;
			slider_1.gameObject.GetComponent<exSprite>().scale = sliderSizeInit - sliderSizeDelta * multiplier;
			
			slider_2.gameObject.transform.position = sliderPositionInit_2 - sliderPositionDelta_2 * multiplier;
			slider_2.gameObject.GetComponent<exSprite>().scale = sliderSizeInit - sliderSizeDelta * multiplier;
			
			slider_3.gameObject.transform.position = sliderPositionInit_3 - sliderPositionDelta_3 * multiplier;
			slider_3.gameObject.GetComponent<exSprite>().scale = sliderSizeInit - sliderSizeDelta * multiplier;
			
		}
		
		// Iterate through each active node
		foreach (NotesScript note in notes) {
			float timeDiff = common.GetTimeDiff(note);
			common.CheckAutoPlay(note, timeDiff);
			common.UpdateNoteState(note, timeDiff);
		}
	}
}

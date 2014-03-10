using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModeSlideScript : MonoBehaviour {
	
	// Positions
	private Vector3 sliderPositionInit	= new Vector3(   0f,  140f, CommonScript.LAYER_SLIDER);
	private Vector3 sliderPositionEnd	= new Vector3(   0f, -150f, CommonScript.LAYER_SLIDER);
	
	private Vector3 notesPositionInit_0	= new Vector3( 120f,  140f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionInit_1	= new Vector3(  40f,  140f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionInit_2	= new Vector3( -40f,  140f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionInit_3	= new Vector3(-120f,  140f, CommonScript.LAYER_NOTES);
	
	private Vector3 notesPositionEnd_0	= new Vector3( 120f, -150f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionEnd_1	= new Vector3(  40f, -150f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionEnd_2	= new Vector3( -40f, -150f, CommonScript.LAYER_NOTES);
	private Vector3 notesPositionEnd_3	= new Vector3(-120f, -150f, CommonScript.LAYER_NOTES);
	
	private Vector3 notesPositionDelta_0, notesPositionDelta_1, notesPositionDelta_2, notesPositionDelta_3;
	private Vector3 sliderPositionDelta;
	
	private const float SLIDER_WIDTH		= 1.0f;
	private const float SLIDER_HEIGHT		= 1.0f;
	
	// Common Resources
	private CommonScript common;
	
	// Tapboxes
	public GameObject tapboxPrefab;
	private TapboxScript slider;
	
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
		GameObject sliderObject = (GameObject)Instantiate(tapboxPrefab);
		slider = sliderObject.GetComponent<TapboxScript>();
		exSprite sprite = sliderObject.GetComponent<exSprite>();
		sprite.scale = new Vector2(SLIDER_WIDTH, SLIDER_HEIGHT);
		sprite.color = new Color(1, 1, 1, 0.75f);
		sliderPositionDelta = sliderPositionInit - sliderPositionEnd;
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
			slider.gameObject.active = false;
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
			slider.gameObject.transform.position = sliderPositionInit - sliderPositionDelta * multiplier;
		}
		
		// Iterate through each active node
		foreach (NotesScript note in notes) {
			float timeDiff = common.GetTimeDiff(note);
			common.CheckAutoPlay(note, timeDiff);
			common.UpdateNoteState(note, timeDiff);			
		}
	}
}

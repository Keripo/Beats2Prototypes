using UnityEngine;
using System.Collections;

public class NotesScript : MonoBehaviour {
	
	// Enums
	public enum NotesState {
		DISABLE,
		ACTIVE,
		HIT,
		MISS,
		REMOVE
	};
	public enum NotesType {
		TAP,
		HOLD,
		SLIDE
	};
	
	// Note data
	public float time;
	public float time_hit;
	public int column;
	public int fraction;
	public NotesType type;
	public NotesState state;
	private exSpriteAnimation anim;
	
	// Hack for ReverseGridScript due to laziness
	public TapboxScript tapbox;
	
	// Awake is called prior to Start()
	void Awake() {
		this.gameObject.tag = Tags.NOTE;
	}
	
	// Use this for initialization
	public void Start() {
		anim = this.gameObject.GetComponent<exSpriteAnimation>();
	}
	
	// Setup
	public void Setup(float time, int column, int fraction, NotesType type) {
		// Init variables
		this.time = time;
		this.column = column;
		this.fraction = fraction;
		this.type = type;
		this.state = NotesState.DISABLE;
	}
		
	// Update is called once per frame
	public void Update() {
	}
	
	// Animation only
	public void PlayHitAnim() {
		anim.Play("NotesHitAnim");
	}
	
	// Animation only
	public void PlayMissAnim() {
		anim.Play("NotesMissAnim");
	}
}

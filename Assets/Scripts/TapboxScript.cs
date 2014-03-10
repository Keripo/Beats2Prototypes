using UnityEngine;
using System.Collections;

public class TapboxScript : MonoBehaviour {
	
	public int column;
	private exSpriteAnimation anim;
	
	// Awake is called prior to Start()
	void Awake() {
		this.gameObject.tag = Tags.TAPBOX;
	}
	
	// Use this for initialization
	void Start() {
		anim = this.gameObject.GetComponent<exSpriteAnimation>();
	}
	
	// Update is called once per frame
	void Update() {
	}
	
	// Setup
	public void Setup(int column) {
		this.column = column;
	}
	
	// Animation only
	public void PlayDownAnim() {
		anim.Play("TapboxTapAnim");
		//anim.Play("TapboxDownAnim");
	}
	
	// Animation only
	public void PlayUpAnim() {
		//anim.Play("TapboxUpAnim");
	}
	
}


using UnityEngine;
using System.Collections;

public class FeedbackStarScript : MonoBehaviour {
	
	public int row, column;
	private exSpriteAnimation anim;
	
	// Awake is called prior to Start()
	void Awake() {
		this.gameObject.tag = Tags.FEEDBACK_STAR;
	}
	
	// Use this for initialization
	void Start () {
		anim = this.gameObject.GetComponent<exSpriteAnimation>();
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public void PlaySelectAnim() {
		anim.Play("Feedback_Star_Select_Anim");
	}
	
	public void PlayDeselectAnim() {
		anim.Play("Feedback_Star_Deselect_Anim");
	}
}

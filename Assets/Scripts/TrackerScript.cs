using UnityEngine;
using System.Collections;

public class TrackerScript : MonoBehaviour {
	
	/*
	// Constants
	private const int PLAYTOMIC_SWFID	= 7625;
	private const string PLAYTOMIC_GUID	= "98bf615385414c93";
	private const string PLAYTOMIC_API	= "e8030926fd484c5e91b65d6262fe8d";
	
	private static bool initialized = false;
	
	*/
	// Use this for initialization
	void Start () {
	/*
		if (!TrackerScript.initialized) {
			Playtomic.Initialize(PLAYTOMIC_SWFID, PLAYTOMIC_GUID, PLAYTOMIC_API);
			Playtomic.Log.View();
			TrackerScript.initialized = true;
		}
	*/
	}
	
	// App loses focus
	void OnApplicationPause(bool pause) {
		if (pause) {
			//Debug.LogError("pause");
		}
	}
	
	// App gains focus
	void OnApplicationFocus(bool focus) {
		if (focus) {
			//Debug.LogError("focus");
			//Playtomic.Log.View();
		}
	}
	
	// Playtomic: https://playtomic.com/api/unity#Analytics
	// Lumos: http://www.uselumos.com/support/docs
	
	public void Counter(string level, string activity) {
		//Playtomic.Log.LevelCounterMetric(activity, level);
		Lumos.Event(string.Format("{0}-{1}", level, activity));
		//Debug.LogError(string.Format("Counter: {0}, {1}", level, activity));
	}
	
	public void Average(string level, string activity, int val) {
		//Playtomic.Log.LevelAverageMetric(activity, level, val);
		Lumos.Event(string.Format("{0}-{1}", level, activity), val);
		//Debug.LogError(string.Format("Average: {0}, {1} = {2}", level, activity, val));
	}
	
	public void Ranged(string level, string activity, int val) {
		//Playtomic.Log.LevelRangedMetric(activity, level, val);
		Lumos.Event(string.Format("{0}-{1}", level, activity), val);
		//Debug.LogError(string.Format("Ranged: {0}, {1} = {2}", level, activity, val));
	}
	
	public void Pause() {
		//Playtomic.Log.Freeze();
	}
	
	public void Resume() {
		//Playtomic.Log.UnFreeze();
		//Playtomic.Log.ForceSend();
	}
}

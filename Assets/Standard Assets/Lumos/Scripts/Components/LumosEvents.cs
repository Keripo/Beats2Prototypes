// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service that allows custom events to be sent.
/// </summary>
public class LumosEvents : MonoBehaviour
{
	/// <summary>
	/// The stored events.
	/// </summary>
	static List<Dictionary<string, object>> events = new List<Dictionary<string, object>>();

	/// <summary>
	/// Names of unique (non-repeated) events that have yet to be recorded by the server.
	/// </summary>
	static List<string> unsentUniqueEvents = new List<string>();

	LumosEvents () {}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	/// <param name="repeatable">Whether this event should only be logged once per player.</param>
	public static void Record (string name, float? value, bool repeatable)
	{
		if (name == null || name == "") {
			Lumos.LogWarning("Name must be sent. Event not recorded.");
			return;
		}

		// If the event should only be logged a maximum of one time, ensure it hasn't been recorded before
		if (!repeatable) {
			if (RecordedUnique(name)) {
				return;
			}

			unsentUniqueEvents.Add(name);
		}

		var parameters = new Dictionary<string, object>() {
			{ "name", name },
			{ "level", Application.loadedLevelName },
		};

		if (value.HasValue) {
			//parameters.Add("value", value.Value);
			parameters.Add("value_str", value.Value.ToString());
		}

		events.Add(parameters);
	}

	/// <summary>
	/// Returns true if an event flagged as not repeating has been recorded by this player.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <returns>Whether the unique event has been recorded.</returns>
	public static bool RecordedUnique (string name)
	{
		var recorded = false;

		// Check if player prefs has recorded the event or if it's still waiting to be sent
		if (PlayerPrefs.HasKey(PlayerPrefsKey(name)) || unsentUniqueEvents.Contains(name)) {
			recorded = true;
		}

		return recorded;

	}

	/// <summary>
	/// Returns the key used to store a unique event in player prefs.
	/// </summary>
	/// <param name="eventName">The name of the event.</param>
	/// <returns>The player prefs key.</returns>
	public static string PlayerPrefsKey (string eventName)
	{
		var key = "lumos_event_" + Application.loadedLevelName + "_" + eventName;
		return key;
	}

	/// <summary>
	/// Sends the events.
	/// </summary>
	public static void Send ()
	{
		if (events.Count == 0) {
			return;
		}

		var parameters = new Dictionary<string, object>() {
			{ "events", events }
		};

		LumosWWW.Send("events.record", parameters, delegate {
			// Save unrepeatable events to player prefs with a timestamp
			foreach (var eventName in unsentUniqueEvents) {
				PlayerPrefs.SetString(PlayerPrefsKey(eventName), System.DateTime.Now.ToString());
			}

			unsentUniqueEvents.Clear();
			events.Clear();
		});
	}

	float levelStartTime;

	void Awake ()
	{
		levelStartTime = Time.time;
		Lumos.Event("Level Started", false);
	}

	void OnLevelWasLoaded ()
	{
		Lumos.Event("Level Completion Time", Time.time - levelStartTime);
		levelStartTime = Time.time;
		Lumos.Event("Level Started", false);
	}
}

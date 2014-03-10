// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service that allows feedback to be collected from the player.
/// A default GUI is provided, but a custom one can be used instead by calling the Record function on submit.
/// </summary>
public class LumosFeedback
{
	public delegate void CloseHandler();
	
	/// <summary>
	/// A callback that triggers when the window is closed.
	/// </summary>
#if !UNITY_FLASH
	public static event CloseHandler windowClosed;
#else
	public static CloseHandler windowClosed;
#endif
	
	/// <summary>
	/// The skin to use for the GUI window.
	/// </summary>
	public static GUISkin skin { get; set; }
	
	const int windowId = 345992; // Random with the hope of being different than the user's window ID
	const int margin = 10;
	static Rect windowRect;
	static bool visible;
	static bool sentMessage;
	
	//static readonly string[] types = { "Feature Request", "Bug", "Question" };
	
	static string email = "";
	static string message = "";
	static string type = "Feature Request";
	
	LumosFeedback () {}
	
	/// <summary>
	/// Displays a window where the player can enter their email address and feedback.
	/// </summary>
	public static void OnGUI ()
	{
		if (!visible) {
			return;
		}
		
		if (skin != null) {
			GUI.skin = skin;	
		}
		
		windowRect = new Rect(margin, margin, Screen.width - (2 * margin), Screen.height - (2 * margin));
		// Register the window
		GUILayout.Window(windowId, windowRect, DisplayWindow, "Leave Feedback");
	}
	
	/// <summary>
	/// Displays the window.
	/// </summary>
	/// <param name="windowID">The window's ID.</param>
	static void DisplayWindow (int windowId)
	{
		GUI.BringWindowToFront(windowId);
		
		GUILayout.BeginHorizontal();
		
			GUILayout.Label("Email (optional)", GUILayout.ExpandWidth(false));
			email = GUILayout.TextField(email, 320);
		
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		
			GUILayout.Label("Type", GUILayout.ExpandWidth(false));
			type = GUILayout.TextField(type);
		
		GUILayout.EndHorizontal();
		
		if (!sentMessage) {
			message = GUILayout.TextArea(message, GUILayout.MinHeight(200));	
		} else {
			GUILayout.BeginHorizontal();
			
				GUILayout.FlexibleSpace();
				GUILayout.Label("Your message has been sent.");	
				GUILayout.FlexibleSpace();
				
			GUILayout.EndHorizontal();
		}
		
		GUILayout.BeginHorizontal();
			
			GUILayout.FlexibleSpace();
			
			if (!sentMessage) {
				if (GUILayout.Button("Cancel")) {
					HideDialog();
				}
			
				if (GUILayout.Button("Send")) {
					Record(message, email, type);
					message = "";
					sentMessage = true;
				}
			} else {
				if (GUILayout.Button("OK")) {
					HideDialog();
					sentMessage = false;
				}	
			}
		
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Shows the feedback window.
	/// </summary>
	public static void ShowDialog ()
	{
		visible = true;
	}
	
	/// <summary>
	/// Hides the feedback window.
	/// </summary>
	public static void HideDialog ()
	{
		// Trigger callback function if one has been specified
		if (windowClosed != null) {
			windowClosed();
		}
		
		visible = false;
	}
	
	/// <summary>
	/// Records a message and sends it immediately.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="email">The player's email address.</param>
	/// <param name="type">The category of feedback.</param>
	public static void Record (string message, string email, string type)
	{
		var parameters = new Dictionary<string, object>() {
			{ "player_id", Lumos.playerId },
			{ "message", message }
		};
		
		if (email != null) {
			parameters.Add("email", email);
		}
		
		if (type != null) {
			parameters.Add("type", type);
		}
		
		LumosWWW.Send("feedback.record", parameters);
	}
}

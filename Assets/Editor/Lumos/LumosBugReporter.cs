// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System;
using System.Net;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Report issues with Lumos. Open from the Help > Lumos > Report a Bug menu.
/// </summary>
public class LumosBugReporter : EditorWindow
{
	static readonly GUIContent emailLabel  = new GUIContent("Email",  "Your email address (optional).");
	static readonly GUIContent issueLabel  = new GUIContent("Issue",  "Details about the problem you're running into.");
	static readonly GUIContent submitLabel = new GUIContent("Submit", "Send your issue.");
	
	static readonly GUIContent sentNotification  = new GUIContent("Bug Report Sent\nThanks!");
	static readonly GUIContent errorNotification = new GUIContent("An Error Occurred\nBug Report Not Sent");
	
	string email = ""; // Optional
	string issue = "";
	
	bool inProgress;
	GUIContent currentNotification;
	
	static bool debug = false;

	void OnGUI ()
	{
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(false));
			GUILayout.Label(emailLabel);
			GUILayout.Label(issueLabel);
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginVertical();
			email = EditorGUILayout.TextField(email);
			issue = EditorGUILayout.TextArea(issue, GUILayout.ExpandHeight(true));
			
			GUI.enabled = issue != "";
			
			if (inProgress) {
				// Draw progress bar
				// This doesn't accurately reflect the request's progress, but it indicates that something is happening
				var rect = GUILayoutUtility.GetRect(submitLabel, GUI.skin.button);
				EditorGUI.ProgressBar(rect, 0.4f, "Sending...");
			} else {
				if (GUILayout.Button(submitLabel)) {
					ReportBug();
				}
			}
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		
		if (currentNotification != null) {
			ShowNotification(currentNotification);
			currentNotification = null;
		}
	}
	
	/// <summary>
	/// Asynchronously sends the bug report to the server.
	/// </summary>
	void ReportBug ()
	{
		var message = "email=" + email + "&issue=" + issue;
		var bytes = Encoding.UTF8.GetBytes(message);
		
		var url = "http://" + (debug ? "localhost:8080" : "www.uselumos.com") + "/report-bug";
		var request = WebRequest.Create(url);
		request.Method = "POST";
		request.ContentLength = bytes.Length;
		request.ContentType = "application/x-www-form-urlencoded";
		
		try {
			var stream = request.GetRequestStream();
			stream.Write(bytes, 0, bytes.Length);
			stream.Close();
			request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
			inProgress = true;
		} catch (Exception e) {
			if (debug) { Debug.LogError("[Lumos] Bug report not sent. " + e.Message); }
			currentNotification = errorNotification;
		}
	}
	
	/// <summary>
	/// Called upon completion of the asynchronous request to the server.
	/// </summary>
	/// <param name="ar">Asynchronous result.</param>
	void ResponseCallback (IAsyncResult ar)
	{
		var request = (WebRequest)ar.AsyncState;
		request.EndGetResponse(ar);
		inProgress = false;
		currentNotification = sentNotification;
	}
}

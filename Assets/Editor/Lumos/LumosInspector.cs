// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using UnityEditor;
using UnityEngine;

/// <summary>
/// A custom inspector for the Lumos game object.
/// </summary>
[CustomEditor(typeof(Lumos))]
public class LumosInspector : Editor
{
	GUIContent secretKeyLabel          = new GUIContent("Secret Key",             "Your application's secret key.");
	GUIContent recordInEditorLabel     = new GUIContent("Record While In Editor", "Send data to Lumos during development.");
	GUIContent recordPresetEventsLabel = new GUIContent("Record Preset Events",   "Record pre-configured events.");
	GUIContent recordErrorsLabel       = new GUIContent("Record Errors",          "Report error messages sent to Unity's console.");
	GUIContent recordWarningsLabel     = new GUIContent("Record Warnings",        "Report warning message sent to Unity's console.");
	GUIContent recordLogsLabel         = new GUIContent("Record Logs",            "Report debug log message sent to Unity's console.");
	
	override public void OnInspectorGUI ()
	{
		var lumos = target as Lumos;
		
		EditorGUIUtility.LookLikeInspector();
		EditorGUI.indentLevel = 1;
		
		lumos.secretKey          = EditorGUILayout.TextField(secretKeyLabel,       lumos.secretKey);
		lumos.runInEditor        = EditorGUILayout.Toggle(recordInEditorLabel,     lumos.runInEditor);
		lumos.recordPresetEvents = EditorGUILayout.Toggle(recordPresetEventsLabel, lumos.recordPresetEvents);
		
		EditorGUILayout.Space();
		
		lumos.recordErrors   = EditorGUILayout.Toggle(recordErrorsLabel,   lumos.recordErrors);
		lumos.recordWarnings = EditorGUILayout.Toggle(recordWarningsLabel, lumos.recordWarnings);
		lumos.recordLogs     = EditorGUILayout.Toggle(recordLogsLabel,     lumos.recordLogs);
	}
}

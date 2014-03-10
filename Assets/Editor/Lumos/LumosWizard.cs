// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using UnityEditor;
using UnityEngine;

/// <summary>
/// A wizard for instantiating the Lumos game object.
/// </summary>
public class LumosWizard : ScriptableWizard
{
	const string prefabPath = "Assets/Standard Assets/Lumos/Lumos.prefab";
	public string secretKey = "";
	
	/// <summary>
	/// Called when the "Create" button is pressed.
	/// </summary>
	void OnWizardCreate ()
	{
		Undo.RegisterSceneUndo("Add Lumos To Scene");
		
		// Instantiate the Lumos object
		var prefab = Resources.LoadAssetAtPath(prefabPath, typeof(GameObject));
		var go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
		go.GetComponent<Lumos>().secretKey = secretKey;
	}
	
	/// <summary>
	/// Called when the "Cancel" button is pressed.
	/// </summary>
	void OnWizardOtherButton ()
	{
		Close();
	}
	
	/// <summary>
	/// UI elements.
	/// </summary>
	void OnWizardUpdate ()
	{
		helpString = "Fill in your application's secret key below.";
	}
}

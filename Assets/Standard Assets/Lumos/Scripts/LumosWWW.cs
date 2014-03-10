// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>
/// Functionality for communicating with Lumos' servers.
/// </summary>
public class LumosWWW
{
	public delegate void SuccessHandler();
	public delegate void ErrorHandler();
	
	static string _url = "http://www.uselumos.com/api/";
	/// <summary>
	/// The Lumos RPC URL.
	/// </summary>
	public static string url {
		private get { return _url; }
		set { _url = value; }
	}

	/// <summary>
	/// Sends data to Lumos' servers.
	/// </summary>
	public static Coroutine Send (string method, Dictionary<string, object> parameters)
	{
		return Lumos.RunRoutine(SendCoroutine(method, parameters, null, null));
	}
	
	/// <summary>
	/// Sends data to Lumos' servers.
	/// </summary>
	/// <param name="successCallback">Callback to run on successful response.</param>
	public static Coroutine Send (string method, Dictionary<string, object> parameters, SuccessHandler successCallback)
	{
		return Lumos.RunRoutine(SendCoroutine(method, parameters, successCallback, null));
	}
	
	/// <summary>
	/// Sends data to Lumos' servers.
	/// </summary>
	/// <param name="successCallback">Callback to run on successful response.</param>
	/// <param name="errorCallback">Callback to run on failed response.</param>
	public static Coroutine Send (string method, Dictionary<string, object> parameters, SuccessHandler successCallback, ErrorHandler errorCallback)
	{
		return Lumos.RunRoutine(SendCoroutine(method, parameters, successCallback, errorCallback));
	}

#if !UNITY_FLASH

	static readonly Hashtable headers = new Hashtable() {
		{ "Content-type", "application/json" }
	};

	/// <summary>
	/// Sends data to Lumos' servers.
	/// </summary>
	/// <param name="successCallback">Callback to run on successful response.</param>
	/// <param name="errorCallback">Callback to run on failed response.</param>
	static IEnumerator SendCoroutine (string method, Dictionary<string, object> parameters, SuccessHandler successCallback, ErrorHandler errorCallback)
	{
		if (Application.isEditor && !Lumos.instance.runInEditor) {
			yield break;
		}
		
		// Skip out early if there's no internet connection
		if (Application.internetReachability == NetworkReachability.NotReachable) {
			if (errorCallback != null) {
				errorCallback();
			}
			
			yield break;
		}
		
		// Generate request
		parameters.Add("app_id", Lumos.appId);
		var json = LumosUtil.Json.Serialize(parameters);
		//var json = LitJson.JsonMapper.ToJson(parameters);
		var postData = Encoding.ASCII.GetBytes(json);
		var www = new WWW(url + method, postData, headers);
		
		// Send info to server
		yield return www;
		Lumos.Log("Request: " + json);
		Lumos.Log("Response: " + www.text);
		
		// Parse the response
		try {
			if (www.error != null) {
				throw new Exception(www.error);
			}

			var response = LumosUtil.Json.Deserialize(www.text) as IDictionary;

			// Display returned info if there is any
			if (response.Count != 0 && response.Contains("result")) {
				var result = response["result"];
				Lumos.Log("Success: " + result);
			}

			if (successCallback != null) {
				successCallback();
			}
		} catch (Exception e) {
			Lumos.LogError("Failure: " + e.Message);
			
			if (errorCallback != null) {
				errorCallback();
			}
		}
	}

#else

	static IEnumerator SendCoroutine (string method, Dictionary<string, object> parameters, SuccessHandler successCallback, ErrorHandler errorCallback) { yield break; }

#endif

}

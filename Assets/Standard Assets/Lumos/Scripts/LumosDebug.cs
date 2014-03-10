// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using UnityEngine;

/// <summary>
/// Replacement Debug functions that the library can use without being picked up by the log callback.
/// </summary>
public partial class Lumos
{
	delegate void MessageLogger (object message);
	
	/// <summary>
	/// Logs a regular debug message.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void Log (object message)
	{
		LogMessage(Debug.Log, message);
	}
	
	/// <summary>
	/// Logs a warning.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void LogWarning (object message)
	{
		LogMessage(Debug.LogWarning, message);
	}
	
	/// <summary>
	/// Logs an error.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static void LogError (object message)
	{
		LogMessage(Debug.LogError, message);
	}
	
	/// <summary>
	/// Logs a message that won't be caught by the LumosLogs service.
	/// </summary>
	/// <param name="logger">The function to send the message to.</param>
	/// <param name="message">The message to log.</param>
	static void LogMessage (MessageLogger logger, object message)
	{
		if (instance == null || !debug) {
			return;
		}
		
		logger("[Lumos] " + message.ToString());
	}
}

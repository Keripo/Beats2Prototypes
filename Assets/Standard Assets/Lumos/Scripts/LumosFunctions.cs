// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

/// <summary>
/// Convenience methods for triggering Lumos functionality.
/// </summary>
public partial class Lumos
{
	#region Events

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	public static void Event (string name)
	{
		LumosEvents.Record(name, null, true);
	}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	public static void Event (string name, int value)
	{
		LumosEvents.Record(name, (float)value, true);
	}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	public static void Event (string name, float value)
	{
		LumosEvents.Record(name, value, true);
	}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="repeatable">Whether this event should only be logged once per player.</param>
	public static void Event (string name, bool repeatable)
	{
		LumosEvents.Record(name, null, repeatable);
	}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	/// <param name="repeatable">Whether this event should only be logged once per player.</param>
	public static void Event (string name, int value, bool repeatable)
	{
		LumosEvents.Record(name, (float)value, repeatable);
	}

	/// <summary>
	/// Records an event.
	/// </summary>
	/// <param name="name">The name of the event.</param>
	/// <param name="value">An arbitrary value to send with the event.</param>
	/// <param name="repeatable">Whether this event should only be logged once per player.</param>
	public static void Event (string name, float value, bool repeatable)
	{
		LumosEvents.Record(name, value, repeatable);
	}

	#endregion

	#region Feedback
	
	/// <summary>
	/// Records a feedback message.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Feedback (string message)
	{
		LumosFeedback.Record(message, null, null);
	}
	
	/// <summary>
	/// Records a feedback message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="email">The player's email address.</param>
	public static void Feedback (string message, string email)
	{
		LumosFeedback.Record(message, email, null);
	}
	
	/// <summary>
	/// Records a feedback message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="email">The player's email address.</param>
	/// <param name="type">The category of feedback.</param>
	public static void Feedback (string message, string email, string type)
	{
		LumosFeedback.Record(message, email, type);
	}
	
	#endregion

	#region Realtime

	/// <summary>
	/// Sends a message or multiple messages to the realtime feed.
	/// </summary>
	/// <param name="message">The message(s) to send.</param>
	public static void Realtime (params string[] message)
	{
		LumosRealtime.Record(message);
	}

	#endregion
}

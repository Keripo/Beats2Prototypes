// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service that sends messages to Lumos' realtime stream.
/// These are not recorded by the service and cannot be viewed afterwards.
/// </summary>
public class LumosRealtime
{
	/// <summary>
	/// Sends messages to the realtime feed.
	/// </summary>
	/// <param name="messages">The messages to send.</param>
	public static void Record (string[] messages)
	{
		if (messages.Length == 0) {
			return;
		}

		var encodedMessages = new List<string>();

		foreach (var message in messages) {
			var encoded = new Dictionary<string, string>() {
				{ "level", Application.loadedLevelName },
				{ "message", message}
			};

			var json = LumosUtil.Json.Serialize(encoded);
			encodedMessages.Add(json);
		}

		var parameters = new Dictionary<string, object>() {
			{ "messages", encodedMessages }
		};

		LumosWWW.Send("realtime.message", parameters);
	}
}

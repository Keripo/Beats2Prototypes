// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// General actions for the app itself.
/// </summary>
public class LumosApp
{
	/// <summary>
	/// Notifies the server that the player is playing.
	/// </summary>
	/// 
	public static void Ping ()
	{
		Ping(false);
	}
	
	/// <summary>
	/// Notifies the server that the player is playing.
	/// </summary>
	/// <param name="sendPlayerInfo">Whether to send info about the player.</param>
	public static void Ping (bool sendPlayerInfo)
	{
		var parameters = new Dictionary<string, object>() {
			{ "player_id", Lumos.playerId },
			{ "lumos_version", Lumos.version.ToString() }
		};
		
		if (sendPlayerInfo) {
			var playerInfo = new Dictionary<string, object>() {
				{ "language", Application.systemLanguage.ToString() }
			};
			
			// Report the domain if the game's deployed on the web
			if (Application.isWebPlayer) {
				playerInfo.Add("domain", Application.absoluteURL);
			}
			
			parameters.Add("player_info", playerInfo);
		}
		
		LumosWWW.Send("app.ping", parameters);
	}
}

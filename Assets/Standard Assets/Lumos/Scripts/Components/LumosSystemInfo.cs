// Copyright (c) 2012 Rebel Hippo Inc. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A service that records the player's software and hardware capabilities (OS, RAM, etc.).
/// </summary>
public class LumosSystemInfo
{

#if !UNITY_IPHONE

	/// <summary>
	/// Sends system information.
	/// </summary>
	public static void Record ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			Lumos.LogWarning("Apple's ToS prevents device data from being sent on iOS");
			return;
		}
		
		var parameters = new Dictionary<string, object>() {
			{ "player_id", Lumos.playerId },
			{ "operating_system", SystemInfo.operatingSystem },
			{ "processor_type", SystemInfo.processorType },
			{ "processor_count", SystemInfo.processorCount },
			{ "system_memory_size", SystemInfo.systemMemorySize },
			{ "graphics_memory_size", SystemInfo.graphicsMemorySize },
			{ "graphics_device_name", SystemInfo.graphicsDeviceName },
			{ "graphics_device_vendor", SystemInfo.graphicsDeviceVendor },
			{ "graphics_device_id", SystemInfo.graphicsDeviceID },
			{ "graphics_device_vendor_id", SystemInfo.graphicsDeviceVendorID },
			{ "graphics_device_version", SystemInfo.graphicsDeviceVersion },
			{ "graphics_shader_level", SystemInfo.graphicsShaderLevel },
			{ "graphics_pixel_fillrate", SystemInfo.graphicsPixelFillrate },
			{ "supports_shadows", SystemInfo.supportsShadows },
			{ "supports_render_textures", SystemInfo.supportsRenderTextures },
			{ "supports_image_effects", SystemInfo.supportsImageEffects }
		};
		
		LumosWWW.Send("system-info.record", parameters);
	}

#else

	public static void Record () {}

#endif

}

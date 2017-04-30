﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.BL
{
	/// <summary>
	/// 
	///		Custom enum (despite there is a same enum in the NAudio package)
	///		because with this enum, it's not necessary to have a dependency 
	///		on the NAudio package from the WPF project.
	/// 
	/// </summary>
	public enum PlayState : byte
	{
		Stopped = 0,
		Paused,
		Playing
	}
}

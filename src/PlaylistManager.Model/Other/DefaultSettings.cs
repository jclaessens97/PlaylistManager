﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.Model;
using static System.Environment;

namespace PlaylistManager.Model.Other
{
	public static class DefaultSettings
	{
		#region Implicit Default Settings

		public static readonly float Volume = 100f;
		public static readonly bool ShuffleEnabled = false;
		public static readonly RepeatMode Repeatmode = RepeatMode.Off;

		#endregion

		#region Explicit Default Settings

		public static readonly string Folder = GetFolderPath(SpecialFolder.MyMusic);
		public static readonly bool IncludeSubdirs = false;
		public static readonly float TimeBetweenSongs = 3f;

		#endregion
	}
}

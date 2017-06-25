using System;
using PlaylistManager.Model.Other;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Contains all settings
	/// Reads settings from settingsfile
	/// Singleton class
	/// </summary>
	public sealed class Settings
	{
		private static volatile Settings instance;
		private static readonly object syncRoot = new Object();

		public static Settings Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new Settings();
						}
					}
				}

				return instance;
			}
		}

		public float Volume { get; private set; }

		public string Folder { get; private set; }
		public bool IncludeSubdirs { get; private set; }

		private Settings()
		{
			Volume = 100; //TODO: read from settings file

			Folder = Globals.DEBUG_MUSIC_FOLDER_PATH; 
			IncludeSubdirs = Globals.DEBUG_INCLUDE_SUBDIRS;
		}

		public void ChangeFolder(string _path)
		{
			//TODO: change folder method
			throw new NotImplementedException();
		}
	}
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlaylistManager.Model.Properties;
using TagLib;
using TagLib.Riff;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Song (same datatypes as defined in the TagLib library)
	/// </summary>
	public class Song
	{
		private bool isPlaying;
		
		public bool IsPlaying
		{
			get => isPlaying;
			set
			{
				if (value.Equals(isPlaying)) return;
				isPlaying = value;
			}
		}

		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public TimeSpan Duration { get; set; }
		public string Path { get; set; }
		public string[] Genres { get; set; }
		public uint? Year { get; set; }
		public uint? TrackNumber { get; set; }
		public IPicture AlbumArt { get; set; }

		public override string ToString()
		{
			return $"{Artist} - {Title} ({Duration:hh\\:mm\\:ss})";
		}
	}
}
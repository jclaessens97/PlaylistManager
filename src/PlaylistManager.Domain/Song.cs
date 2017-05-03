using System;
using TagLib;

namespace PlaylistManager.Domain
{
	/// <summary>
	///     Model that holds all song properties
	///     (same datatypes as defined in the TagLib library)
	/// </summary>
	public class Song
	{
		public string Title { get; set; }
		public string Album { get; set; }
		public TimeSpan Duration { get; set; }
		public string Path { get; set; }
		public string[] Genres { get; set; }
		public string Artist { get; set; }
		public uint? Year { get; set; }
		public uint? TrackNumber { get; set; }
		public IPicture AlbumArt { get; set; }
	}
}
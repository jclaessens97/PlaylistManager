using System;
using System.Collections.Generic;

namespace PlaylistManager.Domain
{
	/// <summary>
	///     Model that holds all playlist properties
	/// </summary>
	public class Playlist
	{
		public string Name { get; set; }
		public List<Song> Songs { get; set; }
		public TimeSpan Duration { get; set; }

		public Playlist(string name)
		{
			Name = name;
		}

		public void AddSong(Song song)
		{
			throw new NotImplementedException();
		}

		public void RemoveSong(Song song)
		{
			throw new NotImplementedException();
		}
	}
}
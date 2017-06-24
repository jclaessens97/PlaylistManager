using System;
using System.Collections.Generic;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Playlist
	/// </summary>
	public class Playlist
	{
		public string Name { get; set; }
		public List<Song> Songs { get; set; }
		public TimeSpan Duration { get; set; }

		public Playlist(string _name)
		{
			Name = _name;
		}

		public void AddSong(Song _song)
		{
			throw new NotImplementedException();
		}

		public void RemoveSong(Song _song)
		{
			throw new NotImplementedException();
		}
	}
}
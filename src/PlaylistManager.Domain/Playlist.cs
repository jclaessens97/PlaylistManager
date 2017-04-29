using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.Domain
{
	/// <summary>
	/// 
	///		Model that holds all playlist properties
	/// 
	/// </summary>
	public class Playlist
	{
		public string Name { get; set; }
		public List<Song> Songs { get; set; }
		public TimeSpan Duration { get; set; }

		public Playlist(string name)
		{
			this.Name = name;
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

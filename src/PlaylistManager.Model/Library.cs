using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TagLib.Flac;
using PlaylistManager.Model.Properties;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Library
	/// Singleton class
	/// </summary>
	public class Library
	{
		#region Attributes

		private static volatile Library instance;
		private static readonly object syncRoot = new Object();

		#endregion

		#region Properties

		public static Library Instance
		{
			get
			{
				if (instance == null)
				{
					lock (syncRoot)
					{
						if (instance == null)
						{
							instance = new Library();
						}
					}
				}

				return instance;
			}
		}

		public List<Song> Songs { get; set; }
		public List<Playlist> Playlists { get; set; }
		public List<Song> NowPlayingList { get; set; }

		#endregion

		private Library()
		{	
			LoadSongs();
		}

		/// <summary>
		/// Load songs from folder (set in settings) into library
		/// </summary>
		public void LoadSongs()
		{
			this.Songs = new List<Song>();

			try
			{
				string[] files;
				if (Settings.Default.IncludeSubdirs)
				{
					files = Directory.GetFiles(Settings.Default.Folder, "*.*", SearchOption.AllDirectories);
				}
				else
				{
					files = Directory.GetFiles(Settings.Default.Folder);
				}

				foreach (var filename in files)
				{
					if (!filename.EndsWith(".mp3")) continue;
					
					//TODO: Remove doubles
					
					TagLib.File file = TagLib.File.Create(filename);
					Song song = new Song()
					{
						IsPlaying = false,

						Artist = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : null,
						Title = file.Tag.Title,
						Album = file.Tag.Album,
						Duration = file.Properties.Duration,
						Path = filename,
						//Genres = file.Tag.Genres, //TODO: issue #8
						Genres = null,
						Year = file.Tag.Year,
						TrackNumber = file.Tag.Track,
						AlbumArt = file.Tag.Pictures.Length > 0 ? new Picture(file.Tag.Pictures[0]) : null
					};

					if (!string.IsNullOrEmpty(song.Artist))
						song.Artist = song.Artist.Trim();

					if (!string.IsNullOrEmpty(song.Title))
						song.Title = song.Title.Trim();

					if (!string.IsNullOrEmpty(song.Album))
						song.Album = song.Album.Trim();

					if (song.Year == 0)
						song.Year = null;

					if (song.TrackNumber == 0)
						song.TrackNumber = null;

					this.Songs.Add(song);
				}

			}
			catch (IOException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		#region NowPlayingList

		/// <summary>
		/// Generate random now playing list
		/// </summary>
		/// <param name="_shuffled">is shuffled or not</param>
		public void GenerateNowPlayingList(bool _shuffled)
		{
			if (!_shuffled)
			{
				NowPlayingList = Songs.ToList();
				return;
			}

			NowPlayingList = new List<Song>(Songs.Count);
			var songsCopy = new List<Song>(Songs);
			Random rnd = new Random();

			while (songsCopy.Count > 0)
			{
				var songToAdd = songsCopy[rnd.Next(songsCopy.Count)];
				NowPlayingList.Add(songToAdd);
				songsCopy.Remove(songToAdd);
			}

			PrintPlayingNowList();
		}

		/// <summary>
		/// Generate now playing list starting from song
		/// </summary>
		/// <param name="_song">song to start with</param>
		/// <param name="_shuffled">is shuffled or not</param>
		public void GenerateNowPlayingList(Song _song, bool _shuffled)
		{
			NowPlayingList = new List<Song>(Songs.Count);
			var songsCopy = new List<Song>(Songs);
			int startIndex = songsCopy.IndexOf(_song);

			NowPlayingList.Add(songsCopy[startIndex]);
			songsCopy.RemoveAt(startIndex);

			if (!_shuffled)
			{
				int songsLeft = songsCopy.Count - startIndex;

				while (songsLeft > 0)
				{
					var songToAdd = songsCopy[startIndex];
					NowPlayingList.Add(songToAdd);
					songsCopy.Remove(songToAdd);

					songsLeft--;
				}

				for (int i = 0; i < startIndex; i++)
				{
					var songToAdd = songsCopy[i];
					NowPlayingList.Add(songToAdd);
				}
			}
			else
			{
				//TODO: repeat once

				Random rnd = new Random();

				while (songsCopy.Count > 0)
				{
					var songToAdd = songsCopy[rnd.Next(songsCopy.Count)];
					NowPlayingList.Add(songToAdd);
					songsCopy.Remove(songToAdd);
				}
			}

			PrintPlayingNowList();
		}

		/// <summary>
		/// Generate now playing list from list of songs
		/// </summary>
		/// <param name="_songs">the list of songs</param>
		/// <param name="_shuffled">is shuffled or not</param>
		public void GenerateNowPlayingList(List<Song> _songs, bool _shuffled)
		{
			if (!_shuffled)
			{
				NowPlayingList = _songs;
				return;
			}

			NowPlayingList = new List<Song>(_songs.Count);
			var songsCopy = _songs;
			Random rnd = new Random();

			while (songsCopy.Count > 0)
			{
				var songToAdd = songsCopy[rnd.Next(songsCopy.Count)];
				NowPlayingList.Add(songToAdd);
				songsCopy.Remove(songToAdd);
			}

			PrintPlayingNowList();
		}

		#endregion

		#region Debug

		[System.Diagnostics.Conditional("DEBUG")]
		private void PrintPlayingNowList()
		{
			//NowPlayingList.ForEach(s => Debug.WriteLine(s));
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Flac;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Library
	/// </summary>
	public class Library
	{
		public Settings Settings { get; }

		public List<Song> Songs { get; set; }
		public List<Playlist> Playlists { get; set; }
		public List<Song> NowPlayingList { get; set; }

		public Library()
		{
			Settings = new Settings();
			
			LoadSongs();
		}

		private void LoadSongs()
		{
			this.Songs = new List<Song>();

			try
			{
				string[] files;
				if (Settings.IncludeSubdirs)
				{
					files = Directory.GetFiles(Settings.Folder, "*.*", SearchOption.AllDirectories);
				}
				else
				{
					files = Directory.GetFiles(Settings.Folder);
				}

				foreach (var filename in files)
				{
					if (!filename.EndsWith(".mp3")) continue;
					
					//TODO: Remove doubles
					
					TagLib.File file = TagLib.File.Create(filename);
					Song song = new Song()
					{
						IsPlaying = false,

						Artist = file.Tag.Performers[0],
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
	}
}

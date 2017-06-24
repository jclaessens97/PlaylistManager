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
		public string Folder { get; }

		public List<Song> Songs { get; set; }
		public List<Playlist> Playlists { get; set; }
		public List<Song> NowPlayingList { get; set; }

		public Library(string _folder, bool _includeSubDirs)
		{
			Folder = _folder;
			LoadSongs(_includeSubDirs);
		}

		private void LoadSongs(bool _includeSubdirs)
		{
			this.Songs = new List<Song>();

			try
			{
				string[] files;
				if (_includeSubdirs)
				{
					files = Directory.GetFiles(Folder, "*.*", SearchOption.AllDirectories);
				}
				else
				{
					files = Directory.GetFiles(Folder);
				}


				uint id = 0;

				foreach (var filename in files)
				{
					if (!filename.EndsWith(".mp3")) continue;

					TagLib.File file = TagLib.File.Create(filename);
					Song song = new Song()
					{
						IsPlaying = false,

						Id = id++,
						Artist = file.Tag.Performers[0],
						Title = file.Tag.Title,
						Album = file.Tag.Album,
						Duration = file.Properties.Duration,
						Path = filename,
						//Genres = file.Tag.Genres,
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

		public void ChangeFolder(string _folder, bool _includeSubdirs)
		{
			throw new NotImplementedException();
		}
	}
}

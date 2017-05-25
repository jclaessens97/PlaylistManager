using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib.Flac;

namespace PlaylistManager.Domain
{
	public class Library
	{
		public string Folder { get; private set; }

		public List<Song> Songs { get; set; }
		public List<Playlist> Playlists { get; set; }
		public List<Song> NowPlayingList { get; set; }

		public Library(string folder, bool includeSubDirs)
		{
			Folder = folder;
			LoadSongs(includeSubDirs);
		}

		private void LoadSongs(bool includeSubdirs)
		{
			this.Songs = new List<Song>();

			try
			{
				string[] files;
				if (includeSubdirs)
				{
					files = Directory.GetFiles(Folder, "*.*", SearchOption.AllDirectories);
				}
				else
				{
					files = Directory.GetFiles(Folder);
				}

				foreach (var filename in files)
				{
					if (!filename.EndsWith(".mp3")) continue;

					TagLib.File file = TagLib.File.Create(filename);
					Song song = new Song()
					{
						Artist = file.Tag.Performers[0].Trim(),
						Title = file.Tag.Title.Trim(),
						Album = file.Tag.Album,
						Duration = file.Properties.Duration,
						Path = filename,
						Genres = file.Tag.Genres,
						Year = file.Tag.Year,
						TrackNumber = file.Tag.Track,
						AlbumArt = file.Tag.Pictures.Length > 0 ? new Picture(file.Tag.Pictures[0]) : null
					};

					this.Songs.Add(song);
				}
			}
			catch (IOException ex)
			{
				Debug.WriteLine(ex.Message);
			}
		}

		public void ChangeFolder(string folder, bool includeSubdirs)
		{
			throw new NotImplementedException();
		}
	}
}

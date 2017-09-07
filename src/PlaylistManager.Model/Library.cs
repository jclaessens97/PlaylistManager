using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PlaylistManager.Model.Other;
using TagLib.Flac;
using PlaylistManager.Model.Properties;

namespace PlaylistManager.Model
{
    /// <summary>
    /// Library 
    /// Singleton pattern
    /// </summary>
	public class Library
	{
        #region Attributes

	    private static Library instance;
        private static readonly object lockObject = new object();

        #endregion

        #region Properties

	    public static Library Instance
	    {
	        get
	        {
	            if (instance == null)
	            {
	                lock (lockObject)
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

        #region Load

        /// <summary>
        /// Load songs from folder (set in settings) asynchronously
        /// </summary>
	    public void LoadSongs()
	    {
            try
	        {
                Songs = new List<Song>();

                string[] files;
	            if (Settings.Default.IncludeSubdirs)
	            {
	                files = Directory.GetFiles(Settings.Default.Folder, "*.*", SearchOption.AllDirectories);
	            }
	            else
	            {
	                files = Directory.GetFiles(Settings.Default.Folder);
	            }

	            Debug.WriteLine("Starting to read files!");
                Parallel.ForEach(files, InitSong);
	            Songs = Songs.OrderBy(s => s.Title).ToList();
                Debug.WriteLine("Finished!");
	        }
	        catch (IOException ex)
	        {
	            Debug.WriteLine(ex);
	        }
	        catch (Exception ex)
	        {
	            Debug.WriteLine(ex);
	        }
	    }

        #endregion

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
		}

        #endregion

        #region Auxilary methods

	    /// <summary>
	    /// Auxilary method to make song object based on filename and add it to the Songs list
	    /// </summary>
	    /// <param name="_filename"></param>
	    private void InitSong(string _filename)
	    {
	        if (!_filename.EndsWith(".mp3")) return;

	        TagLib.File file = TagLib.File.Create(_filename);
	        Song song = new Song()
	        {
	            IsPlaying = false,
	            Artist = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : null,
	            Title = file.Tag.Title,
	            Album = file.Tag.Album,
	            Duration = file.Properties.Duration,
	            Path = _filename,
	            //Genres = file.Tag.Genres, //TODO: issue #8
	            Genres = null,
	            Year = file.Tag.Year,
	            TrackNumber = file.Tag.Track,
	            AlbumArt = file.Tag.Pictures.Length > 0 ? new Picture(file.Tag.Pictures[0]) : null
	        };

	        //Check for empty's & trim the attributes
	        if (!string.IsNullOrWhiteSpace(song.Artist))
	            song.Artist = song.Artist.Trim();

	        if (!string.IsNullOrWhiteSpace(song.Title))
	            song.Title = song.Title.Trim();

	        if (!string.IsNullOrWhiteSpace(song.Album))
	            song.Album = song.Album.Trim();

	        if (song.Year == 0)
	            song.Year = null;

	        if (song.TrackNumber == 0)
	            song.TrackNumber = null;

	        Songs.Add(song);
	    }

        #endregion
    }
}

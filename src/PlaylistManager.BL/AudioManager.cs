using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.Domain;
using TagLib.Flac;

namespace PlaylistManager.BL
{
	public class AudioManager
	{
		private readonly AudioPlayer _audioPlayer;
		private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Ed Sheeran - Trap Queen (cover).mp3"; //<1h
		//private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Chill Mix 2015 (Eric Clapton).mp3"; //>1h

		public Song SongPlaying { get; set; }
		public bool IsPlaying { get; set; }

		public AudioManager()
		{
			_audioPlayer = new AudioPlayer();
		}

		public Song LoadSongFromPath(string path)
		{
			TagLib.File audioFile = TagLib.File.Create(path);
			Song song = new Song
			{
				AlbumArtist = audioFile.Tag.Artists[0],
				Title = audioFile.Tag.Title,
				Album = audioFile.Tag.Album,
				Duration = audioFile.Properties.Duration,
				Path = path,
				Genres = audioFile.Tag.Genres,
				Year = audioFile.Tag.Year,
				TrackNumber = audioFile.Tag.Track,
				AlbumArt = audioFile.Tag.Pictures.Length > 0 ? new Picture(audioFile.Tag.Pictures[0]) : null
			};

			return song;
		}

		public void LoadSong()
		{
			if (_audioPlayer.CurrentSong == null)
			{
				this.SongPlaying = LoadSongFromPath(DEBUG_SONG_PATH);
				_audioPlayer.CurrentSong = this.SongPlaying;
			}
		}

		#region audiocontrols
		public void Play()
		{
			IsPlaying = true;
			_audioPlayer.Play();
		}

		public void Resume()
		{
			IsPlaying = true;
			_audioPlayer.Resume();
		}

		public void Pause()
		{
			IsPlaying = false;
			_audioPlayer.Pause();
		}

		public void Next()
		{
			_audioPlayer.Next();
		}

		public void Prev()
		{
			_audioPlayer.Prev();
		}

		public void Stop()
		{
			SongPlaying = null;
			IsPlaying = false;
			_audioPlayer.Stop();
		}

		public void SetVolume(double newVolume)
		{
			_audioPlayer.SetVolume(newVolume);
		}

		public double Mute()
		{
			return _audioPlayer.Mute();
		}
		#endregion
	}
}

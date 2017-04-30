using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Data;
using NAudio.Wave;
using PlaylistManager.Domain;
using TagLib.Flac;

namespace PlaylistManager.BL
{
	public class AudioManager : INotifyPropertyChanged
	{
		private readonly AudioPlayer _audioPlayer;
		private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Ed Sheeran - Trap Queen (cover).mp3";																									  //private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Chill Mix 2015 (Eric Clapton).mp3"; //>1h

		private double _currentTime;
		private Timer _timer;

		public double CurrentTime
		{
			get { return _currentTime; }
			set
			{
				if (value.Equals(_currentTime)) return;
				_currentTime = value;
				OnPropertyChanged("CurrentTime");
			}
		}
		public Song SongPlaying { get; set; }
		public PlayState State { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

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
			State = PlayState.Playing;

			_timer = new Timer();
			_timer.Interval = 300;
			_timer.Elapsed += Timer_Elapsed;
			_timer.Start();

			_audioPlayer.Play();
		}

		public void ToggleResumePause()
		{
			if (State == PlayState.Paused)
			{
				State = PlayState.Playing;
				_audioPlayer.Resume();
			}
			else
			{
				State = PlayState.Paused;
				_audioPlayer.Pause();
			}
		}

		public void Resume()
		{
			_audioPlayer.Resume();
		}

		public void Pause()
		{
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
			State = PlayState.Stopped;
			SongPlaying = null;
			_audioPlayer.Stop();
		}

		public float GetVolume()
		{
			return _audioPlayer.GetVolume();
		}

		public void SetVolume(double newVolume)
		{
			_audioPlayer.SetVolume(newVolume);
		}

		public double Mute()
		{
			return _audioPlayer.Mute();
		}

		public double GetLengthInSeconds()
		{
			return _audioPlayer.GetLengthInSeconds();
		}

		public double GetPositionInSeconds()
		{
			return CurrentTime;
		}

		public void SetPosition(double position)
		{
			_audioPlayer.SetPosition(position);
		}
		#endregion

		#region events
		private void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (State == PlayState.Playing)
			{
				CurrentTime = _audioPlayer.GetPositionInSeconds();
			}
		}
		#endregion
	}
}

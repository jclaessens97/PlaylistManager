using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;
using PlaylistManager.Domain;
using TagLib;
using Picture = TagLib.Flac.Picture;

namespace PlaylistManager.BL
{
	/// <summary>
	///     Class that interacts between WPF and backend classes.
	///     Delegates methods to the right class
	/// </summary>
	public class Manager : INotifyPropertyChanged
	{
		private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Ed Sheeran - Trap Queen (cover).mp3";
		//private const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Chill Mix 2015 (Eric Clapton).mp3"; //>1h
		private const string DEBUG_MUSIC_FOLDER_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\";

		#region audioplayer declarations
		private readonly AudioPlayer _audioPlayer;

		private double _currentTime;
		private Timer _timer;

		private static readonly IDictionary RepeatModeMap = new Dictionary<RepeatMode, RepeatMode>
		{
			{ RepeatMode.Off, RepeatMode.Once },
			{ RepeatMode.Once, RepeatMode.On },
			{ RepeatMode.On, RepeatMode.Off }
		};

		public double CurrentTime
		{
			get => _currentTime;
			set
			{
				if (value.Equals(_currentTime)) return;
				_currentTime = value;
				OnPropertyChanged("CurrentTime");
			}
		}
		public Song SongPlaying { get; set; }
		public PlayState State { get; set; }
		public bool ShuffleEnabled { get; set; }
		public RepeatMode RepeatMode { get; set; }
		#endregion

		private readonly Library _library;
		public Song Song { get; set; }

		public Manager()
		{
			_library = new Library(DEBUG_MUSIC_FOLDER_PATH, true);
			_audioPlayer = new AudioPlayer();
			Song = LoadSongFromPath(DEBUG_SONG_PATH);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#region library

		public bool CheckLibrary()
		{
			if (_library.Folder == string.Empty || _library.Songs.Count == 0)
			{
				return false;
			}

			return true;
		}

		public List<Song> GetLibrarySongs()
		{
			return _library.Songs;
		}

		#endregion

		#region audioPlayer

		public Song LoadSongFromPath(string path)
		{
			var audioFile = File.Create(path);

			var song = new Song
			{
				Artist = audioFile.Tag.Performers[0],
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
				SongPlaying = LoadSongFromPath(DEBUG_SONG_PATH);
				_audioPlayer.CurrentSong = SongPlaying;
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

		public void ToggleRepeat()
		{
			RepeatMode = (RepeatMode)RepeatModeMap[RepeatMode];
		}

		#endregion

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (State == PlayState.Playing)
				CurrentTime = _audioPlayer.GetPositionInSeconds();
		}

		#endregion

		#region events

		private void OnPropertyChanged(string name)
		{
			var handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}
}
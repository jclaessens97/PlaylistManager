using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using PlaylistManager.Domain;


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
		public Song CurrentSong { get; set; }
		public PlayState State { get; set; }
		public bool ShuffleEnabled { get; set; }
		public RepeatMode RepeatMode { get; set; }
		#endregion

		private readonly Library _library;

		public Manager()
		{
			_library = new Library(DEBUG_MUSIC_FOLDER_PATH, true);
			_audioPlayer = new AudioPlayer();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#region library

		public bool CheckLibrary()
		{
			if (_library.Folder == string.Empty || _library.Songs.Count == 0)
				return false;

			return true;
		}

		public List<Song> GetLibrarySongs()
		{
			return _library.Songs;
		}

		#endregion

		#region audioPlayer

		#region audiocontrols

		public void Play()
		{
			Debug.Assert(CurrentSong != null);

			GeneratePlayingNowList();

			State = PlayState.Playing;

			_timer = new Timer();
			_timer.Interval = 300;
			_timer.Elapsed += Timer_Elapsed;
			_timer.Start();

			_audioPlayer.Play(CurrentSong);
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
			if (RepeatMode == RepeatMode.Off)
			{
				if (HasNext())
				{
					int index = _library.NowPlayingList.IndexOf(CurrentSong) + 1;
					CurrentSong = _library.NowPlayingList[index];
				}
				else
				{
					Stop();
				}
			}
			else if (RepeatMode == RepeatMode.On)
			{
				int index;

				if (HasNext())
				{
					index = _library.NowPlayingList.IndexOf(CurrentSong) + 1;
				}
				else
				{
					index = 0;
				}

				CurrentSong = _library.NowPlayingList[index];
			}

			_audioPlayer.Stop();
			_audioPlayer.Play(CurrentSong);

			//Debug.WriteLine("Now playing:" + CurrentSong);
			//Debug.WriteLine("Index: " + _library.NowPlayingList.IndexOf(CurrentSong));
		}

		public void Prev()
		{
			if (RepeatMode == RepeatMode.Off)
			{
				if (HasPrev())
				{
					int index = _library.NowPlayingList.IndexOf(CurrentSong) - 1;
					CurrentSong = _library.NowPlayingList[index];
				}
				else
				{
					Stop();
				}
			}
			else if (RepeatMode == RepeatMode.On)
			{
				int index;

				if (HasPrev())
				{
					index = _library.NowPlayingList.IndexOf(CurrentSong);
				}
				else
				{
					index = _library.NowPlayingList.Count - 1;
				}

				CurrentSong = _library.NowPlayingList[index];
			}

			_audioPlayer.Stop();
			_audioPlayer.Play(CurrentSong);

			Debug.WriteLine("Now playing:" + CurrentSong);
			Debug.WriteLine("Index: " + _library.NowPlayingList.IndexOf(CurrentSong));
		}

		public void Stop()
		{
			State = PlayState.Stopped;
			CurrentSong = null;
			_library.NowPlayingList = null;
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

		public void SetPosition(double position)
		{
			_audioPlayer.SetPosition(position);
		}

		public void ToggleShuffle()
		{
			ShuffleEnabled = !ShuffleEnabled;

			if (ShuffleEnabled && (State == PlayState.Playing || State == PlayState.Paused))
			{
				_library.NowPlayingList.ShuffleFromCurrent(CurrentSong);
			}
			else if (ShuffleEnabled && State == PlayState.Stopped)
			{
				_library.NowPlayingList.ShuffleAll();
			}
			else
			{
				_library.NowPlayingList = _library.Songs;
			}
		}

		public void ToggleRepeat()
		{
			RepeatMode = (RepeatMode)RepeatModeMap[RepeatMode];
		}

		#endregion

		#region auxilary

		public bool HasNext()
		{
			int index = _library.NowPlayingList.IndexOf(CurrentSong);

			if (RepeatMode == RepeatMode.Once) return false;
			return index < _library.NowPlayingList.Count - 1;
		}



		public bool HasPrev()
		{
			int index = _library.NowPlayingList.IndexOf(CurrentSong);

			if (RepeatMode == RepeatMode.Once) return false;
			return index > 0;
		}

		#endregion

		#region other

		private void GeneratePlayingNowList()
		{
			_library.NowPlayingList = new List<Song> { CurrentSong };

			if (RepeatMode == RepeatMode.Once) return;

			if (ShuffleEnabled)
			{
				Random rnd = new Random();

				for (int i = 0; i < _library.Songs.Count; i++)
				{
					int index = rnd.Next(0, _library.Songs.Count - i);

					Song song = _library.Songs.Where(s => s != CurrentSong).ToList()[index];
					_library.NowPlayingList.Add(song);
				}

			}
			else
			{
				int index = _library.Songs.IndexOf(CurrentSong);

				if (index < _library.Songs.Count - 1)
				{
					for (int i = index + 1; i < _library.Songs.Count; i++)
					{
						Song song = _library.Songs[i];
						_library.NowPlayingList.Add(song);
					}

					for (int i = 0; i < index; i++)
					{
						Song song = _library.Songs[i];
						_library.NowPlayingList.Add(song);
					}
				}
				else
				{
					for (int i = 0; i < _library.Songs.Count - 1; i++)
					{
						Song song = _library.Songs[i];
						_library.NowPlayingList.Add(song);
					}
				}
			}

			//_library.NowPlayingList.ForEach(x => Debug.WriteLine(x));
		}

		public Song GetRandomSong()
		{
			//TODO
			return _library.Songs[0];
		}

		public double GetLengthInSeconds()
		{
			return CurrentSong.Duration.TotalSeconds;
		}

		public double GetPositionInSeconds()
		{
			return CurrentTime;
		}

		#endregion

		#endregion

		#region events

		private void OnPropertyChanged(string name)
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (State == PlayState.Playing)
				CurrentTime = _audioPlayer.GetPositionInSeconds();
		}

		#endregion
	}
}
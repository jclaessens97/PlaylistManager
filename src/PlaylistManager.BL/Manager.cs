using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using PlaylistManager.Domain;
using PlaylistManager.Domain.Annotations;


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

		private Song _currentSong;
		private double _currentTime;
		private Timer _timer;

		private static readonly IDictionary RepeatModeMap = new Dictionary<RepeatMode, RepeatMode>
		{
			{RepeatMode.Off, RepeatMode.Once},
			{RepeatMode.Once, RepeatMode.On},
			{RepeatMode.On, RepeatMode.Off}
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
		public Song CurrentSong
		{
			get => _currentSong;
			set
			{
				if (value != null && value.Equals(_currentSong)) return;

				if (_currentSong != null)
					_currentSong.IsPlaying = false;

				_currentSong = value;
				OnPropertyChanged();
			} 
		}
		public Song SelectedSong { get; set; }
		public PlayState State { get; set; }
		public bool ShuffleEnabled { get; set; }
		public RepeatMode RepeatMode { get; set; }
		#endregion

		public Library Library { get; }

		public Manager()
		{
			Library = new Library(DEBUG_MUSIC_FOLDER_PATH, true);
			_audioPlayer = new AudioPlayer();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		#region library

		public bool CheckLibrary()
		{
			if (Library.Folder == string.Empty || Library.Songs.Count == 0)
				return false;

			return true;
		}

		public void SetNowPlayingList(List<Song> songs)
		{
			Library.NowPlayingList = songs.ToList();
		}

		#endregion

		#region audioPlayer


		#region auxilary

		public bool HasNext()
		{
			int index = Library.NowPlayingList.IndexOf(CurrentSong);

			if (RepeatMode == RepeatMode.Once) return false;
			return index < Library.NowPlayingList.Count - 1;
		}

		public bool HasPrev()
		{
			int index = Library.NowPlayingList.IndexOf(CurrentSong);

			if (RepeatMode == RepeatMode.Once) return false;
			return index > 0;
		}

		#endregion

		#region other
		private void GeneratePlayingNowList()
		{
			Library.NowPlayingList = new List<Song> { CurrentSong };

			if (RepeatMode == RepeatMode.Once) return;

			if (ShuffleEnabled)
			{
				Random rnd = new Random();

				for (int i = 0; i < Library.Songs.Count; i++)
				{
					int index = rnd.Next(0, Library.Songs.Count - i);

					Song song = Library.Songs.Where(s => s != CurrentSong).ToList()[index];
					Library.NowPlayingList.Add(song);
				}

			}
			else
			{
				int index = Library.Songs.IndexOf(CurrentSong);

				if (index < Library.Songs.Count - 1)
				{
					for (int i = index + 1; i < Library.Songs.Count; i++)
					{
						Song song = Library.Songs[i];
						Library.NowPlayingList.Add(song);
					}

					for (int i = 0; i < index; i++)
					{
						Song song = Library.Songs[i];
						Library.NowPlayingList.Add(song);
					}
				}
				else
				{
					for (int i = 0; i < Library.Songs.Count - 1; i++)
					{
						Song song = Library.Songs[i];
						Library.NowPlayingList.Add(song);
					}
				}
			}

			//Library.NowPlayingList.ForEach(x => Debug.WriteLine(x));
		}

		public Song GetRandomSong()
		{
			Random rnd = new Random();
			return Library.Songs[rnd.Next(Library.Songs.Count - 1)];
		}

		public double GetLengthInSeconds()
		{
			return CurrentSong.Duration.TotalSeconds;
		}

		public double GetPositionInSeconds()
		{
			return CurrentTime;
		}

		public bool CurrentSongIsFinished()
		{
			return _audioPlayer.IsFinished();
		}
		#endregion

		#endregion

		#region events
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

			if (propertyName == nameof(CurrentSong) && CurrentSong != null)
			{
				CurrentSong.IsPlaying = true;
			}
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (State == PlayState.Playing)
				CurrentTime = _audioPlayer.GetPositionInSeconds();
     	}

		#endregion

		#region debug
		[System.Diagnostics.Conditional("DEBUG")]
		public void PrintNowPlayingList()
		{
			foreach (var song in Library.NowPlayingList)
			{
				Debug.WriteLine(song);
			}
		}
		#endregion
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.Model;
using System.Timers;
using System.Windows.Input;
using PlaylistManager.BL;
using PlaylistManager.Model.Other;

//TODO: Add scrollwheel event to volumebar
namespace PlaylistManager.ViewModel
{
	public class AudioplayerPresenter : ObservableObject
	{
		#region Attributes

		private readonly IDictionary repeatModeMap = new Dictionary<RepeatMode, RepeatMode>
		{
			{RepeatMode.Off, RepeatMode.Once},
			{RepeatMode.Once, RepeatMode.On},
			{RepeatMode.On, RepeatMode.Off}
		};
		private readonly AudioPlayer audioPlayer;
		private Song currentSong;
		private Song selectedSong; //???
		private PlayState state;
		private bool shuffleEnabled;
		private RepeatMode repeatMode;

		private string title;
		private string artist;
		private string album;

		private double currentSeconds;
		private string formattedCurrentSeconds;
		private double totalSeconds;
		private string formattedTotalSeconds;

		private float volume;

		private Timer updateTimer;

		#endregion
		
		#region Properties

		public string Title
		{
			get => title;
			set
			{
				if (value == null || value == title) return;
				title = value;
				RaisePropertyChangedEvent(nameof(Title));
			}
		}
		public string Artist
		{
			get => artist;
			set
			{
				if (value == null || value == artist) return;
				artist = value;
				RaisePropertyChangedEvent(nameof(Artist));
			}
		}
		public string Album
		{
			get => album;
			set
			{
				if (value == null || value == album) return;
				album = value;
				RaisePropertyChangedEvent(nameof(Album));
			}
		}

		public double CurrentSeconds
		{
			get => currentSeconds;
			set
			{
				currentSeconds = value;
				RaisePropertyChangedEvent(nameof(CurrentSeconds));
				FormattedCurrentSeconds = Tools.FormatDuration(currentSeconds);
			}
		}
		public string FormattedCurrentSeconds
		{
			get => formattedCurrentSeconds;
			set
			{
				formattedCurrentSeconds = value;
				RaisePropertyChangedEvent(nameof(FormattedCurrentSeconds));
			}
		}
		public double TotalSeconds
		{
			get => totalSeconds;
			set
			{
				totalSeconds = value;
				RaisePropertyChangedEvent(nameof(TotalSeconds));
				FormattedTotalSeconds = Tools.FormatDuration(totalSeconds);
			}
		}
		public string FormattedTotalSeconds
		{
			get => formattedTotalSeconds;
			set
			{
				formattedTotalSeconds = value;
				RaisePropertyChangedEvent(nameof(FormattedTotalSeconds));
			}
		}

		public float Volume
		{
			get => volume;
			set
			{
				volume = value;
				SetVolume();
				RaisePropertyChangedEvent(nameof(Volume));
				Debug.WriteLine("Volume: " + volume);
			}
		}

		public Song CurrentSong
		{
			get => currentSong;
			set
			{
				if (currentSong == value) return;
				currentSong = value; 
				RaisePropertyChangedEvent(nameof(CurrentSong));

				if (currentSong != null)
				{
					Title = CurrentSong.Title;
					Artist = CurrentSong.Artist;
					Album = CurrentSong.Album;
				}
			}
		}
		public Song SelectedSong
		{
			get => selectedSong;
			set
			{
				selectedSong = value;
				RaisePropertyChangedEvent(nameof(SelectedSong));
			}
		}
		public PlayState State
		{
			get => state;
			set
			{
				state = value;
				RaisePropertyChangedEvent(nameof(State));
			}
		}
		public bool ShuffleEnabled
		{
			get => shuffleEnabled;
			set
			{
				shuffleEnabled = value;
				RaisePropertyChangedEvent(nameof(ShuffleEnabled));
			}
		}
		public RepeatMode RepeatMode
		{
			get => repeatMode;
			set
			{
				repeatMode = value;
				RaisePropertyChangedEvent(nameof(RepeatMode));
			}
		}

		#endregion

		#region Commands

		//Navigation
		public ICommand NextCommand => new DelegateCommand(Next);
		public ICommand ToggleResumePauseCommand => new DelegateCommand(ToggleResumePause);
		public ICommand StopCommand => new DelegateCommand(Stop);
		public ICommand PrevCommand => new DelegateCommand(Prev);
		public ICommand ToggleShuffleCommand => new DelegateCommand(ToggleShuffle);
		public ICommand ToggleRepeatCommand => new DelegateCommand(ToggleRepeat);

		//Volume
		public ICommand ToggleMuteCommand => new DelegateCommand(ToggleMute);

		#endregion

		public AudioplayerPresenter()
		{
			audioPlayer = new AudioPlayer();
			State = PlayState.Stopped;
			Volume = 100; //TODO init with volume from last time (settings)
			Reset();
		}

		#region Navigation control commands

		private void ToggleResumePause()
		{
			switch (State)
			{
				case PlayState.Stopped:
					Debug.WriteLine("Play!");
					State = PlayState.Playing;
					Start();
					break;
				case PlayState.Paused:
					Debug.WriteLine("Pause!");
					State = PlayState.Playing;
					Resume();
					break;
				case PlayState.Playing:
					Debug.WriteLine("Play!");
					State = PlayState.Paused;
					Pause();
					break;
			}
		}

		private void Start()
		{
			//TODO: remove (get song from random or selected song in grid)
			CurrentSong = Globals.DEBUG_SONG;
			Debug.Assert(CurrentSong != null);

			//TODO implement play
			//			if (Library.NowPlayingList == null)
			//				GeneratePlayingNowList();

			updateTimer = new Timer();
			updateTimer.Interval = 250;
			updateTimer.Elapsed += UpdateTimer_Elapsed;
			updateTimer.Start();

			audioPlayer.SetVolume(100, false);
			audioPlayer.Play(CurrentSong);
			TotalSeconds = audioPlayer.GetLengthInSeconds();
		}

		private void Resume()
		{
			audioPlayer.Resume();
		}

		private void Pause()
		{
			audioPlayer.Pause();
		}

		private void Stop()
		{
			Debug.WriteLine("Stop!");

			if (State == PlayState.Stopped) return;
			State = PlayState.Stopped;
			audioPlayer.Stop();

			Reset();
		}

		private void Next()
		{
			Debug.WriteLine("Next");
			//TODO: implement next
			//			if (RepeatMode == RepeatMode.Off)
			//			{
			//				if (HasNext())
			//				{
			//					int index = Library.NowPlayingList.IndexOf(CurrentSong) + 1;
			//					CurrentSong = Library.NowPlayingList[index];
			//				}
			//				else
			//				{
			//					Stop();
			//				}
			//			}
			//			else if (RepeatMode == RepeatMode.On)
			//			{
			//				int index;
			//
			//				if (HasNext())
			//				{
			//					index = Library.NowPlayingList.IndexOf(CurrentSong) + 1;
			//				}
			//				else
			//				{
			//					index = 0;
			//				}
			//
			//				CurrentSong = Library.NowPlayingList[index];
			//			}
			//
			//			_audioPlayer.Stop();
			//			_audioPlayer.Play(CurrentSong);

			//Debug.WriteLine("Now playing:" + CurrentSong);
			//Debug.WriteLine("Index: " + Library.NowPlayingList.IndexOf(CurrentSong));
		}

		private void Prev()
		{
			Debug.WriteLine("Prev");

			//TODO: implement prev
			//			if (RepeatMode == RepeatMode.Off)
			//			{
			//				if (HasPrev())
			//				{
			//					int index = Library.NowPlayingList.IndexOf(CurrentSong) - 1;
			//					CurrentSong = Library.NowPlayingList[index];
			//				}
			//				else
			//				{
			//					Stop();
			//				}
			//			}
			//			else if (RepeatMode == RepeatMode.On)
			//			{
			//				int index;
			//
			//				if (HasPrev())
			//				{
			//					index = Library.NowPlayingList.IndexOf(CurrentSong);
			//				}
			//				else
			//				{
			//					index = Library.NowPlayingList.Count - 1;
			//				}
			//
			//				CurrentSong = Library.NowPlayingList[index];
			//			}
			//
			//			_audioPlayer.Stop();
			//			_audioPlayer.Play(CurrentSong);
			//
			//			Debug.WriteLine("Now playing:" + CurrentSong);
			//			Debug.WriteLine("Index: " + Library.NowPlayingList.IndexOf(CurrentSong));
		}
		
		private void ToggleShuffle()
		{
			ShuffleEnabled = !ShuffleEnabled;
			Debug.WriteLine("Shuffle is " + ShuffleEnabled);

			if (ShuffleEnabled)
			{
				if (State == PlayState.Playing || State == PlayState.Paused)
				{
					//Library.NowPlayingList.ShuffleFromCurrent(CurrentSong);
				} else if (State == PlayState.Stopped)
				{
					//Library.NowPlayingList.ShuffleAll();
				}
			}
			else
			{
				//SetNowPlayingList(Library.Songs);
			}
		}

		private void ToggleRepeat()
		{
			RepeatMode = (RepeatMode)repeatModeMap[RepeatMode];

#if DEBUG
			switch (repeatMode)
			{
				case RepeatMode.Off: Debug.WriteLine("Repeat Off");break;
				case RepeatMode.On: Debug.WriteLine("Repeat On");break;
				case RepeatMode.Once: Debug.WriteLine("Repeat Once");break;
			}
#endif
		}

		public void Seek()
		{
			audioPlayer.Seek(currentSeconds);
		}

		#endregion

		#region Volume controls

		public void SetVolume()
		{
			audioPlayer.SetVolume(volume, State == PlayState.Playing);
		}

		public void ToggleMute()
		{
			Debug.WriteLine("Mute");

			if (CurrentSong == null) return;
			Volume = audioPlayer.ToggleMute();
		}

		#endregion

		#region Auxilary

		public bool HasNext()
		{
			//TODO: implement HasNext()
//			int index = Library.NowPlayingList.IndexOf(CurrentSong);
//
//			if (RepeatMode == RepeatMode.Once) return false;
//			return index < Library.NowPlayingList.Count - 1;

			return false;
		}

		public bool HasPrev()
		{
			//TODO: implement HasPrev
//			int index = Library.NowPlayingList.IndexOf(CurrentSong);
//
//			if (RepeatMode == RepeatMode.Once) return false;
//			return index > 0;

			return false;
		}

		private void Reset()
		{
			Title = string.Empty;
			Artist = string.Empty;
			Album = string.Empty;
			CurrentSong = null;

			CurrentSeconds = 0;
			TotalSeconds = 0;
		}

		#endregion

		private void UpdateTimer_Elapsed(object _sender, ElapsedEventArgs _e)
		{
			CurrentSeconds = audioPlayer.GetPositionInSeconds();
		}
	}
}

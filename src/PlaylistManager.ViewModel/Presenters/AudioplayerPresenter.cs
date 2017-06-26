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
using PlaylistManager.ViewModel.Other;

//TODO: Add scrollwheel event to volumebar
namespace PlaylistManager.ViewModel.Presenters
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
		private readonly Library library;
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
				OnVolumeChanged(EventArgs.Empty);
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
				OnSongChanged(EventArgs.Empty);

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
				OnStateChanged(EventArgs.Empty);
			}
		}
		public bool ShuffleEnabled
		{
			get => shuffleEnabled;
			set
			{
				shuffleEnabled = value;
				RaisePropertyChangedEvent(nameof(ShuffleEnabled));
				OnShuffleChanged(EventArgs.Empty);
			}
		}
		public RepeatMode RepeatMode
		{
			get => repeatMode;
			set
			{
				repeatMode = value;
				RaisePropertyChangedEvent(nameof(RepeatMode));
				OnRepeatChanged(EventArgs.Empty);
			}
		}

		#endregion

		#region Commands

		//Navigation
		public ICommand PrevCommand => new DelegateCommand(Prev);
		public ICommand ToggleResumePauseCommand => new DelegateCommand(ToggleResumePause);
		public ICommand StopCommand => new DelegateCommand(Stop);
		public ICommand NextCommand => new DelegateCommand(Next);
		public ICommand ToggleShuffleCommand => new DelegateCommand(ToggleShuffle);
		public ICommand ToggleRepeatCommand => new DelegateCommand(ToggleRepeat);

		//Volume
		public ICommand ToggleMuteCommand => new DelegateCommand(ToggleMute);

		#endregion

		#region Events

		public event EventHandler SongChanged;
		public event EventHandler StateChanged;
		public event EventHandler ShuffleChanged;
		public event EventHandler RepeatChanged;
		public event EventHandler VolumeChanged;

		private void OnSongChanged(EventArgs _e)
		{
			EventHandler handler = SongChanged;
			handler?.Invoke(CurrentSong, _e);
		}
		private void OnStateChanged(EventArgs _e)
		{
			EventHandler handler = StateChanged;
			handler?.Invoke(State, _e);
		}
		private void OnShuffleChanged(EventArgs _e)
		{
			EventHandler handler = ShuffleChanged;
			handler?.Invoke(ShuffleEnabled, _e);
		}
		private void OnRepeatChanged(EventArgs _e)
		{
			EventHandler handler = RepeatChanged;
			handler?.Invoke(RepeatMode, _e);
			handler?.Invoke(State, _e); //Call 2nd time with state this time to see if playing or not
		}
		private void OnVolumeChanged(EventArgs _e)
		{
			EventHandler handler = VolumeChanged;
			handler?.Invoke(Volume, _e);
		}

		#endregion

		public AudioplayerPresenter()
		{
			Reset();

			audioPlayer = new AudioPlayer();
			library = Library.Instance;
			State = PlayState.Stopped;
			Volume = Settings.Instance.Volume;
		}

		#region Navigation control commands

		private void ToggleResumePause()
		{
			switch (State)
			{
				case PlayState.Stopped:
					Debug.WriteLine("Play!");
					Start();
					break;
				case PlayState.Paused:
					Debug.WriteLine("Play!");
					Resume();
					break;
				case PlayState.Playing:
					Debug.WriteLine("Pause!");
					Pause();
					break;
			}
		}

		private void Start()
		{
			State = PlayState.Playing;
			
			if (CurrentSong == null)
			{
				library.GenerateNowPlayingList(shuffleEnabled);
				CurrentSong = library.NowPlayingList.First();
			}

			Debug.Assert(library.NowPlayingList != null);
			Debug.Assert(CurrentSong != null);

			updateTimer = new Timer();
			updateTimer.Interval = 250;
			updateTimer.Elapsed += UpdateTimer_Elapsed;
			updateTimer.Start();

			audioPlayer.SetVolume(volume, false);
			audioPlayer.Play(CurrentSong);
			TotalSeconds = audioPlayer.GetLengthInSeconds();
		}

		internal void Start(Song _song)
		{
			State = PlayState.Playing;
			library.GenerateNowPlayingList(_song, shuffleEnabled);
			CurrentSong = library.NowPlayingList.First();
			Start();
		}

		internal void Start(List<Song> _songs)
		{
			State = PlayState.Playing;
			library.GenerateNowPlayingList(_songs, shuffleEnabled);
			CurrentSong = library.NowPlayingList.First();
			Start();
		}

		private void Resume()
		{
			State = PlayState.Playing;
			audioPlayer.Resume();
		}

		private void Pause()
		{
			State = PlayState.Paused;
			audioPlayer.Pause();
		}

		private void Stop()
		{
			Debug.WriteLine("Stop!");

			if (State == PlayState.Stopped) return;
			State = PlayState.Stopped;

			updateTimer.Stop();
			updateTimer.Dispose();
			updateTimer = null;

			CurrentSong = null;
			audioPlayer.Stop();

			Reset();
		}

		public void Next()
		{
			Debug.WriteLine("Next");

			if (HasNext())
			{
				int index = library.NowPlayingList.IndexOf(CurrentSong) + 1;
				Stop();
				CurrentSong = library.NowPlayingList[index];
				Start();
			}
			else
			{
				Stop();
			}

			Debug.WriteLine("Now playing:" + CurrentSong);
			Debug.WriteLine("Index: " + library.NowPlayingList.IndexOf(CurrentSong));
		}

		private void Prev()
		{
			Debug.WriteLine("Prev");

			if (HasPrev())
			{
				int index = library.NowPlayingList.IndexOf(CurrentSong) - 1;
				Stop();
				CurrentSong = library.NowPlayingList[index];
				Start();
			}
			else
			{
				Stop(); 
			}

			Debug.WriteLine("Now playing:" + CurrentSong);
			Debug.WriteLine("Index: " + library.NowPlayingList.IndexOf(CurrentSong));
		}

		private void ToggleShuffle()
		{
			ShuffleEnabled = !ShuffleEnabled;
			Debug.WriteLine("Shuffle is " + ShuffleEnabled);

			if (shuffleEnabled)
			{
				if (State == PlayState.Playing || State == PlayState.Paused)
				{
					library.GenerateNowPlayingList(CurrentSong, true);
				}
			}
			else
			{
				if (State == PlayState.Playing || State == PlayState.Paused)
				{
					library.GenerateNowPlayingList(CurrentSong, false);
				}
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

		private void UpdateTimer_Elapsed(object _sender, ElapsedEventArgs _e)
		{
			CurrentSeconds = audioPlayer.GetPositionInSeconds();
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
			if (RepeatMode == RepeatMode.Once) return false;

			if (RepeatMode == RepeatMode.Off)
			{
				int index = library.NowPlayingList.IndexOf(CurrentSong);

				if (index < library.NowPlayingList.Count - 1)
					return true;
				else
					return false;
			}
			else if (RepeatMode == RepeatMode.On)
			{
				return true;
			}

			return false; //should never reach this
		}

		public bool HasPrev()
		{
			if (RepeatMode == RepeatMode.Once) return false;

			if (RepeatMode == RepeatMode.Off)
			{
				int index = library.NowPlayingList.IndexOf(CurrentSong);

				if (index > 0)
					return true;
				else
					return false;
			}
			else if (RepeatMode == RepeatMode.On)
			{
				return true;
			}

			return false; //should never reach this
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
	}
}

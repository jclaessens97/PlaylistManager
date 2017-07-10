using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.Other;
using Timer = System.Timers.Timer;


namespace PlaylistManager.ViewModel.ViewModels
{
    public sealed class AudioPlayerControlViewModel : ObservableObject
    {
        #region Attributes

        private static volatile AudioPlayerControlViewModel instance;
        private static readonly object syncRoot = new object();

        private IAudioPlayerControl audioPlayerControl;

        private readonly IDictionary repeatModeMap = new Dictionary<RepeatMode, RepeatMode>
        {
            {RepeatMode.Off, RepeatMode.Once},
            {RepeatMode.Once, RepeatMode.On},
            {RepeatMode.On, RepeatMode.Off}
        };
        private readonly AudioPlayer audioPlayer;
        private readonly SettingsControlViewModel settingsViewModel;

        private Song currentSong;
        private PlayState state;
        private bool shuffleEnabled;
        private RepeatMode repeatMode;
        private float volume;

        private string title;
        private string artist;
        private string album;

        private double currentSeconds;
        private string formattedCurrentSeconds;
        private double totalSeconds;
        private string formattedTotalSeconds;

        private bool hasNext;
        private bool hasPrev;

        private Timer updateTimer;

        #endregion

        #region Properties

        public static AudioPlayerControlViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new AudioPlayerControlViewModel();
                        }
                    }
                }

                return instance;
            }
        }

        public IAudioPlayerControl AudioPlayerControl
        {
            get => audioPlayerControl;
            set
            {
                audioPlayerControl = value;
                LoadImplicits();
            }
        }

        public Song CurrentSong
        {
            get => currentSong;
            set
            {
                currentSong = value;
                RaisePropertyChangedEvent(nameof(CurrentSong));
                OnCurrentSongChanged();
            }
        }
        public PlayState State
        {
            get => state;
            set
            {
                state = value;
                RaisePropertyChangedEvent(nameof(State));
                OnPlayStateChanged();
            }
        }
        public bool ShuffleEnabled
        {
            get => shuffleEnabled;
            set
            {
                shuffleEnabled = value;
                RaisePropertyChangedEvent(nameof(ShuffleEnabled));
                OnShuffleChanged();
            }
        }
        public RepeatMode RepeatMode   
        {
            get => repeatMode;
            set
            {
                repeatMode = value;
                RaisePropertyChangedEvent(nameof(RepeatMode));
                OnRepeatChanged();
            }
        }
        public float Volume
        {
            get => volume;
            set
            {
                volume = value;
                RaisePropertyChangedEvent(nameof(Volume));
                OnVolumeChanged();
            }
        }

        public string Title
        {
            get => title;
            set
            {
                title = value;
                RaisePropertyChangedEvent(nameof(Title));
            }
        }
        public string Artist
        {
            get => artist;
            set
            {
                artist = value;
                RaisePropertyChangedEvent(nameof(Artist));
            }
        }
        public string Album
        {
            get => album;
            set
            {
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

                if (currentSeconds >= totalSeconds && updateTimer != null)
                    updateTimer.Enabled = false;

                OnCurrentSecondsChanged();
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

        public bool HasNext
        {
            get => hasNext;
            set
            {
                hasNext = value;
                RaisePropertyChangedEvent(nameof(HasNext));
            }
        }
        public bool HasPrev {
            get => hasPrev;
            set
            {
                hasPrev = value;
                RaisePropertyChangedEvent(nameof(HasPrev));
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

        //Slider
        public ICommand PreviewMouseDownCommand
        {
            get
            {
                //Pause song on mouse down while sliding
                var previewMouseDownCommand = new RelayCommand((arg) => ToggleResumePause());
                return previewMouseDownCommand;
            }
        }
        public ICommand PreviewMouseUpCommand
        {
            get
            {
                //Continue playing song on location slider is left when mouse is up
                var previewMouseUpCommand = new DelegateCommand(ContinueAfterSeek);
                return previewMouseUpCommand;
            }
        }

        #endregion

        #region Events

        private void OnCurrentSongChanged()
        {
            AudioPlayerControl.ToggleEnable(CurrentSong != null);
            
        }
        private void OnPlayStateChanged()
        {
            switch (State)
            {
                case PlayState.Stopped:
                    AudioPlayerControl.ToggleEnable(false);
                    DispatcherTools.InvokeDispatcher(AudioPlayerControl.PlayPauseIcon,
                        () => AudioPlayerControl.PlayPauseIcon.Kind = PackIconKind.Play);
                    break;

                case PlayState.Paused:
                    AudioPlayerControl.ToggleEnable(true);
                    DispatcherTools.InvokeDispatcher(AudioPlayerControl.PlayPauseIcon,
                        () => AudioPlayerControl.PlayPauseIcon.Kind = PackIconKind.Play);
                    break;

                case PlayState.Playing:
                    AudioPlayerControl.ToggleEnable(true);
                    DispatcherTools.InvokeDispatcher(AudioPlayerControl.PlayPauseIcon,
                        () => AudioPlayerControl.PlayPauseIcon.Kind = PackIconKind.Pause);
                    break;
            }
        }
        private void OnShuffleChanged()
        {
            if (ShuffleEnabled)
            {
                AudioPlayerControl.ShuffleIcon.Kind = PackIconKind.Shuffle;
            }
            else
            {
                AudioPlayerControl.ShuffleIcon.Kind = PackIconKind.ShuffleDisabled;
            }
        }
        private void OnRepeatChanged()
        {
            switch (RepeatMode)
            {
                case RepeatMode.Off:
                    AudioPlayerControl.RepeatIcon.Kind = PackIconKind.RepeatOff;
                    break;
                case RepeatMode.Once:
                    AudioPlayerControl.RepeatIcon.Kind = PackIconKind.RepeatOnce;
                    break;
                case RepeatMode.On:
                    AudioPlayerControl.RepeatIcon.Kind = PackIconKind.Repeat;
                    break;
            }
        }
        private void OnCurrentSecondsChanged()
        {
            if (CurrentSeconds >= TotalSeconds && CurrentSeconds > 0)
            {
                int waitInMillis = (int) (settingsViewModel.TimeBetweenSongs * 1000);
                Debug.WriteLine("Wait for " + waitInMillis + " ms");
                var autoEvent = new AutoResetEvent(false);
                var timeBetweenSongsChecker = new TimeBetweenSongChecker();
                var timer = new System.Threading.Timer(timeBetweenSongsChecker.CheckTime, autoEvent, waitInMillis, (int)(waitInMillis * 0.25));
                autoEvent.WaitOne();
                timer.Dispose();

                Debug.WriteLine("Continue");
                Next();
            }
        }
        private void OnVolumeChanged()
        {
            //update model
            if (audioPlayer != null && CurrentSong != null)
                audioPlayer.SetVolume(volume, State == PlayState.Playing);

            //update gui
            if (Volume < 1)
            {
                AudioPlayerControl.VolumeIcon.Kind = PackIconKind.VolumeOff;
            }
            else if (Volume <= 33)
            {
                AudioPlayerControl.VolumeIcon.Kind = PackIconKind.VolumeLow;
            }
            else if (Volume <= 66)
            {
                AudioPlayerControl.VolumeIcon.Kind = PackIconKind.VolumeMedium;
            }
            else if (Volume <= 100)
            {
                AudioPlayerControl.VolumeIcon.Kind = PackIconKind.VolumeHigh;
            }
        }

        /// <summary>
        /// Timer thread to update current seconds of the song
        /// </summary>
        /// <param name="_sender"></param>
        /// <param name="_e"></param>
        private void UpdateTimer_Elapsed(object _sender, ElapsedEventArgs _e)
        {
            CurrentSeconds = audioPlayer.GetPositionInSeconds();
        }

        #endregion

        private AudioPlayerControlViewModel()
        {
            audioPlayer = new AudioPlayer();
            settingsViewModel = SettingsControlViewModel.Instance;
            
            //Needed to initialize bindings
            Reset();
        }

        #region Init

        /// <summary>
        /// Load implicit settings when starting audioplayercontrol
        /// Called in setter of audioplayercontrol
        /// Implicit settings:
        ///		- Volume
        ///		- ShuffleEnabled
        ///		- RepeatMode
        /// </summary>
        private void LoadImplicits()
        {
            Volume = settingsViewModel.Volume;
            ShuffleEnabled = settingsViewModel.ShuffleEnabled;
            RepeatMode = settingsViewModel.RepeatMode;
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Switch between pause & play depending on the current playstate
        /// </summary>
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

        /// <summary>
        /// Start playing song & from playlist depending on shufflestate
        /// If song is not set generate play now list & set current song
        /// </summary>
        private void Start()
        {
            var library = LibraryControlViewModel.Instance.Library;
            
            State = PlayState.Playing;

            if (CurrentSong == null)
            {
                library.GenerateNowPlayingList(shuffleEnabled);
                CurrentSong = library.NowPlayingList.First();
            }

            Debug.Assert(library.NowPlayingList != null);
            Debug.Assert(CurrentSong != null);

            Title = CurrentSong.Title;
            Artist = CurrentSong.Artist;
            Album = CurrentSong.Album;

            updateTimer = new Timer();
            updateTimer.Interval = 250;
            updateTimer.Elapsed += UpdateTimer_Elapsed;
            updateTimer.Start();

            HasNextSong();
            HasPrevSong();

            CurrentSong.IsPlaying = true;
            audioPlayer.SetVolume(volume, false);
            audioPlayer.Play(CurrentSong);
            TotalSeconds = audioPlayer.GetLengthInSeconds();
        }

        /// <summary>
        /// Start playing selected song
        /// calls Start() when list is generated & current song is set
        /// Called when double click on song or when a song is selected & play is pressed
        /// </summary>
        /// <param name="_song"></param>
        internal void Start(Song _song)
        {
            if (CurrentSong != null && State == PlayState.Playing)
            {
                Stop();
            }

            var library = LibraryControlViewModel.Instance.Library;

            library.GenerateNowPlayingList(_song, shuffleEnabled);
            CurrentSong = library.NowPlayingList.First();
            Start();
        }

        /// <summary>
        /// Start playing a list of selected songs
        /// calls Start() when list is generated & current song is set
        /// Called when multiple songs are selected & play is pressed
        /// TODO list of songs
        /// </summary>
        /// <param name="_songs"></param>
        internal void Start(List<Song> _songs)
        {
            var library = LibraryControlViewModel.Instance.Library;

            library.GenerateNowPlayingList(_songs, shuffleEnabled);
            CurrentSong = library.NowPlayingList.First();
            Start();
        }

        /// <summary>
        /// Continue playing
        /// </summary>
        private void Resume()
        {
            State = PlayState.Playing;
            updateTimer.Enabled = true;
            audioPlayer.Resume();
        }

        /// <summary>
        /// Pause song
        /// </summary>
        private void Pause()
        {
            State = PlayState.Paused;
            updateTimer.Enabled = false;
            audioPlayer.Pause();
        }

        /// <summary>
        /// Stop song
        /// When stop is clicked or end of list is reached (without repeatmode)
        /// </summary>
        private void Stop()
        {
            Debug.WriteLine("Stop!");

            if (State == PlayState.Stopped) return;
            State = PlayState.Stopped;

            CurrentSong.IsPlaying = false;
            HasPrev = false;
            HasNext = false;

            updateTimer.Stop();
            updateTimer.Dispose();
            updateTimer = null;

            CurrentSong = null;
            audioPlayer.Stop();

            Reset();
        }

        /// <summary>
        /// Stop song before changing to a new song in the list
        /// (Called when navigating to songs (next or prev))
        /// </summary>
        private void StopBetween()
        {
            Debug.WriteLine("Stop between!");

            if (State == PlayState.Stopped) return;

            updateTimer.Stop();
            updateTimer.Dispose();
            updateTimer = null;

            CurrentSong.IsPlaying = false;
            CurrentSong = null;

            audioPlayer.Stop();
        }

        /// <summary>
        /// Next song in list if possible,
        /// else stop
        /// </summary>
        public void Next()
        {
            Debug.WriteLine("Next");
            var library = LibraryControlViewModel.Instance.Library;
            
            if (HasNextSong())
            {
                int index = library.NowPlayingList.IndexOf(CurrentSong) + 1;

                if (index >= library.NowPlayingList.Count)
                {
                    index = 0;
                }

                StopBetween();
                CurrentSong = library.NowPlayingList[index];
                Start();
            }
            else
            {
                if (RepeatMode == RepeatMode.Once)
                {
                    Song s = CurrentSong;
                    StopBetween();
                    Start(s);
                }

                Stop();
            }

            Debug.WriteLine("Now playing:" + CurrentSong);
            Debug.WriteLine("Index: " + library.NowPlayingList.IndexOf(CurrentSong));
        }

        /// <summary>
        /// Prev song in list if possible,
        /// else stop
        /// </summary>
        private void Prev()
        {
            Debug.WriteLine("Prev");
            var library = LibraryControlViewModel.Instance.Library;
            
            if (HasPrevSong())
            {
                int index = library.NowPlayingList.IndexOf(CurrentSong) - 1;

                if (index < 0)
                {
                    index = library.NowPlayingList.Count - 1;
                }

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

        /// <summary>
        /// Toggles between shuffle states
        /// When shuffle changes, change playing now list depending of the shufflestate
        /// </summary>
        private void ToggleShuffle()
        {
            ShuffleEnabled = !ShuffleEnabled;
            Debug.WriteLine("Shuffle is " + ShuffleEnabled);
            var library = LibraryControlViewModel.Instance.Library;

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

            HasNextSong();
            HasPrevSong();
        }

        /// <summary>
        /// Toggles between repeatmode states
        /// </summary>
        private void ToggleRepeat()
        {
            RepeatMode = (RepeatMode)repeatModeMap[RepeatMode];

            HasNextSong();
            HasPrevSong();

#if DEBUG
            switch (repeatMode)
            {
                case RepeatMode.Off: Debug.WriteLine("Repeat Off"); break;
                case RepeatMode.On: Debug.WriteLine("Repeat On"); break;
                case RepeatMode.Once: Debug.WriteLine("Repeat Once"); break;
            }
#endif
        }

        /// <summary>
        /// Scroll throug song using songslider
        /// </summary>
        public void Seek()
        {
            audioPlayer.Seek(currentSeconds);
        }

        /// <summary>
        /// Fires when user stops dragging the timeslider
        /// </summary>
        private void ContinueAfterSeek()
        {
            Seek();
            ToggleResumePause();
        }

        #endregion

        #region Volume

        /// <summary>
        /// Set volume
        /// </summary>
        public void SetVolume()
        {
            audioPlayer.SetVolume(volume, State == PlayState.Playing);
        }

        /// <summary>
        /// Mute volume
        /// </summary>
        public void ToggleMute()
        {
            Debug.WriteLine("Mute");
            if (CurrentSong == null) return;
            Volume = audioPlayer.ToggleMute();
        }

        #endregion

        #region Auxilary

        /// <summary>
        /// Checks if its possible to play a next song
        /// And sets property
        /// </summary>
        /// <returns></returns>
        public bool HasNextSong()
        {
            if (RepeatMode == RepeatMode.Once) HasNext = false;

            if (RepeatMode == RepeatMode.Off)
            {
                var library = LibraryControlViewModel.Instance.Library;
                int index = library.NowPlayingList.IndexOf(CurrentSong);

                if (index < library.NowPlayingList.Count - 1)
                    HasNext = true;
                else
                    HasNext = false;
            }
            else if (RepeatMode == RepeatMode.On)
            {
                HasNext = true;
            }

            return HasNext;
        }

        /// <summary>
        /// Checks if its possible to play a prev song
        /// And sets property
        /// </summary>
        /// <returns></returns>
        public bool HasPrevSong()
        {
            if (RepeatMode == RepeatMode.Once) HasPrev = false;

            if (RepeatMode == RepeatMode.Off)
            {
                var library = LibraryControlViewModel.Instance.Library;
                int index = library.NowPlayingList.IndexOf(CurrentSong);

                if (index > 0)
                    HasPrev = true;
                else
                    HasPrev = false;
            }
            else if (RepeatMode == RepeatMode.On)
            {
                HasPrev = true;
            }

            return HasPrev;
        }

        /// <summary>
        /// Resets properties when song stops or at the beginning
        /// </summary>
        private void Reset()
        {
            Title = string.Empty;
            Artist = string.Empty;
            Album = string.Empty;

            CurrentSeconds = 0;
            TotalSeconds = 0;
        }

        #endregion
    }
}

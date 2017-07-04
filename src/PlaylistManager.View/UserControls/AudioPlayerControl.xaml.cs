using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Presenters;


namespace PlaylistManager.View.UserControls
{
	/// <summary>
	/// Interaction logic for AudioPlayerControl.xaml
	/// Changes visual icons based on the event that happens
	/// Except for SliderTime events: they are used to navigate through the song via the slider
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{
		#region Attributes

		private AudioplayerPresenter audioplayerPresenter;
		private SettingsPresenter settingsPresenter;

		#endregion

		public AudioPlayerControl()
		{
			InitializeComponent();
			ToggleEnable(false);
		}

		#region Event Handlers

		private void SliderTime_OnPreviewMouseDown(object _sender, MouseButtonEventArgs _e)
		{
			var presenter = DataContext as AudioplayerPresenter;
			if (presenter == null) return;

			//Pause song on mouse down while sliding
			if (presenter.ToggleResumePauseCommand.CanExecute(null))
			{
				presenter.ToggleResumePauseCommand.Execute(null);
			}
		}
		private void SliderTime_OnPreviewMouseUp(object _sender, MouseButtonEventArgs _e)
		{
			var presenter = DataContext as AudioplayerPresenter;
			if (presenter == null) return;

			presenter.Seek();

			if (presenter.ToggleResumePauseCommand.CanExecute(null))
			{
				presenter.ToggleResumePauseCommand.Execute(null);
			}
		}

		#endregion

		#region PropertyChanged Events

		/// <summary>
		/// Called in constructor of the main window
		/// After datacontext is set to the presenter
		/// Registers all needed events to the methods below
		/// </summary>
		public void RegisterEvents()
		{
			audioplayerPresenter = DataContext as AudioplayerPresenter;

			if (audioplayerPresenter != null)
			{
				audioplayerPresenter.CurrentSecondsChanged += OnCurrentSecondsChanged;
				audioplayerPresenter.SongChanged += OnSongChanged;
				audioplayerPresenter.StateChanged += OnStateChanged;
				audioplayerPresenter.ShuffleChanged += OnShuffleChanged;
				audioplayerPresenter.RepeatChanged += OnRepeatChanged;
				audioplayerPresenter.VolumeChanged += OnVolumeChanged;
			}
		}

		/// <summary>
		/// Fired each time current seconds changes
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnCurrentSecondsChanged(object _sender, EventArgs _e)
		{
			if (_sender is double)
			{
				double currentSeconds = (double) _sender;
				double totalSeconds = audioplayerPresenter.TotalSeconds;

				if (currentSeconds >= totalSeconds && currentSeconds > 0)
				{
					int waitInMillis = (int) (settingsPresenter.TimeBetweenSongs * 1000);
					Debug.WriteLine("Wait for " + waitInMillis + "ms");
					var autoEvent = new AutoResetEvent(false);
					var timeBetweenSongsChecker = new TimeBetweenSongChecker();
					var timer = new Timer(timeBetweenSongsChecker.CheckTime, autoEvent, waitInMillis, (int)(waitInMillis * 0.25));

					autoEvent.WaitOne();
					timer.Dispose();

					Debug.WriteLine("Continue");
					audioplayerPresenter.Next();
				}
			}
		}

		/// <summary>
		/// Fired when song changes
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnSongChanged(object _sender, EventArgs _e)
		{
			if (_sender is Song)
			{
				ToggleEnable(true);
			}
			else
			{
				ToggleEnable(false);
			}
		}

		/// <summary>
		/// Fired when playstate changes
		/// (Stopped, Paused, Playing)
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnStateChanged(object _sender, EventArgs _e)
		{
			if (_sender is PlayState )
			{
				PlayState state = (PlayState) _sender;

				switch (state)
				{
					case PlayState.Stopped:
						ToggleEnable(false);
						ChangeIcon(PlayPauseIcon, PackIconKind.Play);
						break;
					case PlayState.Paused:
						ToggleEnable(true);
						ChangeIcon(PlayPauseIcon, PackIconKind.Play);
						break;
					case PlayState.Playing:
						ToggleEnable(true);
						ChangeIcon(PlayPauseIcon, PackIconKind.Pause);
						break;
				}
			}
		}

		/// <summary>
		/// Fired when shuffle state changes
		/// (True, False)
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnShuffleChanged(object _sender, EventArgs _e)
		{
			if (_sender is bool)
			{
				bool shuffleEnabled = (bool)_sender;

				if (shuffleEnabled)
				{
					ShuffleIcon.Kind = PackIconKind.Shuffle;
				}
				else
				{
					ShuffleIcon.Kind = PackIconKind.ShuffleDisabled;
				}
			}
		}

		/// <summary>
		/// Fired when repeatmode changes
		/// (Off, Once, On)
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnRepeatChanged(object _sender, EventArgs _e)
		{
			if (_sender is RepeatMode)
			{
				RepeatMode mode = (RepeatMode)_sender;

				switch (mode)
				{
					case RepeatMode.Off:
						RepeatIcon.Kind = PackIconKind.RepeatOff;
						break;
					case RepeatMode.Once:
						RepeatIcon.Kind = PackIconKind.RepeatOnce;
						break;
					case RepeatMode.On:
						RepeatIcon.Kind = PackIconKind.Repeat;
						break;
				}
			}

			if (_sender is PlayState)
			{
				OnStateChanged(_sender, _e);
			}
		}

		/// <summary>
		/// Fired each time volume changes
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
		private void OnVolumeChanged(object _sender, EventArgs _e)
		{
			if (_sender is float)
			{
				float volume = (float) _sender;

				if (volume < 1)
				{
					VolumeIcon.Kind = PackIconKind.VolumeOff;
				}
				else if (volume <= 33)
				{
					VolumeIcon.Kind = PackIconKind.VolumeLow;
				}
				else if (volume <= 66)
				{
					VolumeIcon.Kind = PackIconKind.VolumeMedium;
				}
				else if (volume <= 100)
				{
					VolumeIcon.Kind = PackIconKind.VolumeHigh;
				}
			}
		}

		#endregion

		#region Auxilary

		/// <summary>
		/// Enables/disables all buttons according to state
		/// </summary>
		/// <param name="_state"></param>
		private void ToggleEnable(bool _state)
		{
			ToggleEnableControl(btnStop, _state);
			ToggleEnableControl(sliderTime, _state);
			ToggleEnableControl(btnVolume, _state);
			ToggleEnableControl(sliderVolume, _state);

			if (audioplayerPresenter != null && audioplayerPresenter.CurrentSong != null)
			{
				ToggleEnableControl(btnPrev, audioplayerPresenter.HasPrev());
				ToggleEnableControl(btnNext, audioplayerPresenter.HasNext());
			}
			else
			{
				ToggleEnableControl(btnPrev, false);
				ToggleEnableControl(btnNext, false);
			}
		}

		/// <summary>
		/// Auxilary method to change the enabled attribute of the control
		/// First checks if the thread has access to the control, 
		/// if not, invoke it from there
		/// </summary>
		/// <param name="_control"></param>
		/// <param name="_state"></param>
		private void ToggleEnableControl(Control _control, bool _state)
		{
			if (_control.Dispatcher.CheckAccess())
			{
				_control.IsEnabled = _state;
			}
			else
			{
				_control.Dispatcher.Invoke(() => _control.IsEnabled = false);
			}
		}

		/// <summary>
		/// Changes icon (source) to specified icon (target)
		/// </summary>
		/// <param name="_icon">(source)</param>
		/// <param name="_iconKind">(target)</param>
		private void ChangeIcon(PackIcon _icon, PackIconKind _iconKind)
		{
			if (_icon.Dispatcher.CheckAccess())
			{
				_icon.Kind = _iconKind;
			}
			else
			{
				_icon.Dispatcher.Invoke(() => _icon.Kind = _iconKind);
			}
		}

		#endregion

		#region Other

		/// <summary>
		/// Load implicit settings when starting window
		/// Called in constructor of main window
		/// Implicit settings:
		///		- Volume
		///		- ShuffleEnabled
		///		- RepeatMode
		/// </summary>
		/// <param name="_settingsPresenter"></param>
		public void LoadImplicits(SettingsPresenter _settingsPresenter)
		{
			this.settingsPresenter = _settingsPresenter;
			var presenter = DataContext as AudioplayerPresenter;

			if (presenter != null)
			{
				presenter.Volume = _settingsPresenter.Volume;
				presenter.ShuffleEnabled = _settingsPresenter.ShuffleEnabled;
				presenter.RepeatMode = _settingsPresenter.RepeatMode;
			}
		}

		#endregion
	}
}
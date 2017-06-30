using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using PlaylistManager.ViewModel;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Other;
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
		private AudioplayerPresenter audioplayerPresenter;
		private SettingsPresenter settingsPresenter;

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
					var timeBetweenSongsChecker = new TimeBetweenSongChecker(1);
					var timer = new Timer(timeBetweenSongsChecker.CheckTime, autoEvent, waitInMillis, (int)(waitInMillis * 0.25));

					autoEvent.WaitOne();
					timer.Dispose();

					Debug.WriteLine("Continue");
					audioplayerPresenter.Next();
				}
			}
		}
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

		private void ToggleEnable(bool _state)
		{
			//TODO: cleanup

			if (btnStop.Dispatcher.CheckAccess())
			{
				btnStop.IsEnabled = _state;
			}
			else
			{
				btnStop.Dispatcher.Invoke(() => btnStop.IsEnabled = _state);
			}

			if (sliderTime.Dispatcher.CheckAccess())
			{
				sliderTime.IsEnabled = _state;
			}
			else
			{
				sliderTime.Dispatcher.Invoke(() => sliderTime.IsEnabled = _state);
			}

			if (btnVolume.Dispatcher.CheckAccess())
			{
				btnVolume.IsEnabled = _state;
			}
			else
			{
				btnVolume.Dispatcher.Invoke(() => btnVolume.IsEnabled = _state);
			}

			if (sliderVolume.Dispatcher.CheckAccess())
			{
				sliderVolume.IsEnabled = _state;
			}
			else
			{
				sliderVolume.Dispatcher.Invoke(() => sliderVolume.IsEnabled = _state);
			}

			if (audioplayerPresenter != null && audioplayerPresenter.CurrentSong != null)
			{
				if (btnPrev.Dispatcher.CheckAccess())
				{
					btnPrev.IsEnabled = audioplayerPresenter.HasPrev();
				}
				else
				{
					btnPrev.Dispatcher.Invoke(() => btnPrev.IsEnabled = audioplayerPresenter.HasPrev());
				}

				if (btnNext.Dispatcher.CheckAccess())
				{
					btnNext.IsEnabled = audioplayerPresenter.HasNext();
				}
				else
				{
					btnNext.Dispatcher.Invoke(() => btnNext.IsEnabled = audioplayerPresenter.HasNext());
				}
			}
			else
			{
				if (btnPrev.Dispatcher.CheckAccess())
				{
					btnPrev.IsEnabled = false;
				}
				else
				{
					btnPrev.Dispatcher.Invoke(() => btnPrev.IsEnabled = false);
				}

				if (btnNext.Dispatcher.CheckAccess())
				{
					btnNext.IsEnabled = false;
				}
				else
				{
					btnNext.Dispatcher.Invoke(() => btnNext.IsEnabled = false);
				}
			}
		}

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

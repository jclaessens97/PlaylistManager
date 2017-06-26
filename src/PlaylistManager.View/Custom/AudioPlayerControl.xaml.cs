using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
using PlaylistManager.BL;
using PlaylistManager.ViewModel;
using PlaylistManager.Model;
using PlaylistManager.ViewModel.Presenters;


namespace PlaylistManager.View.Custom
{
	/// <summary>
	/// Interaction logic for AudioPlayerControl.xaml
	/// Changes visual icons based on the event that happens
	/// Except for SliderTime events: they are used to navigate through the song via the slider
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{


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
			var presenter = DataContext as AudioplayerPresenter;

			if (presenter != null)
			{
				presenter.StateChanged += OnStateChanged;
				presenter.ShuffleChanged += OnShuffleChanged;
				presenter.RepeatChanged += OnRepeatChanged;
				presenter.VolumeChanged += OnVolumeChanged;
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
						PlayPauseIcon.Kind = PackIconKind.Play;
						break;
					case PlayState.Paused:
						ToggleEnable(true);
						PlayPauseIcon.Kind = PackIconKind.Play;
						break;
					case PlayState.Playing:
						ToggleEnable(true);
						PlayPauseIcon.Kind = PackIconKind.Pause;
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
			btnStop.IsEnabled = _state;
			sliderTime.IsEnabled = _state;
			btnVolume.IsEnabled = _state;
			sliderVolume.IsEnabled = _state;

			if (DataContext != null)
			{
				var presenter = DataContext as AudioplayerPresenter;

				if (presenter != null)
				{
					btnPrev.IsEnabled = presenter.HasPrev();
					btnNext.IsEnabled = presenter.HasNext();
				}
			}
			else
			{
				btnPrev.IsEnabled = false;
				btnNext.IsEnabled = false;
			}
		}

		#endregion
	}
}

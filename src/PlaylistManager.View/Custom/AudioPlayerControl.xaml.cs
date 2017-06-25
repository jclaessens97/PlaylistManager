using System;
using System.Collections.Generic;
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

		private void BtnPausePlay_OnClick(object _sender, RoutedEventArgs _e)
		{
			ToggleEnable(true);

			if (PlayPauseIcon.Kind == PackIconKind.Pause)
			{
				PlayPauseIcon.Kind = PackIconKind.Play;
			}
			else
			{
				PlayPauseIcon.Kind = PackIconKind.Pause;
			}
		}

		private void BtnStop_OnClick(object _sender, RoutedEventArgs _e)
		{
			PlayPauseIcon.Kind = PackIconKind.Play;

			ToggleEnable(false);
		}

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

		private void BtnShuffle_OnClick(object _sender, RoutedEventArgs _e)
		{
			var presenter = DataContext as AudioplayerPresenter;
			if (presenter == null) return;

			//Inverse of what happens in presenter, because presenter changes after this icon
			if (!presenter.ShuffleEnabled)
			{
				//ShuffleIcon.Kind = PackIconKind.ShuffleVariant;
				ShuffleIcon.Kind = PackIconKind.Shuffle;	//CHOOSE
			}
			else
			{
				ShuffleIcon.Kind = PackIconKind.ShuffleDisabled;
			}
		}

		private void BtnRepeat_OnClick(object _sender, RoutedEventArgs _e)
		{
			var presenter = DataContext as AudioplayerPresenter;
			if (presenter == null) return;

			//Jump one forward because repeat mode is changed in presenter after the icon
			switch (presenter.RepeatMode)
			{
				case RepeatMode.Off:
					RepeatIcon.Kind = PackIconKind.RepeatOnce;
					break;
				case RepeatMode.On:
					RepeatIcon.Kind = PackIconKind.RepeatOff;
					break;
				case RepeatMode.Once:
					RepeatIcon.Kind = PackIconKind.Repeat;
					break;
			}
		}

		private void SliderVolume_OnValueChanged(object _sender, RoutedPropertyChangedEventArgs<double> _e)
		{
			var presenter = DataContext as AudioplayerPresenter;
			if (presenter == null) return;

			if (presenter.Volume < 1)
			{
				VolumeIcon.Kind = PackIconKind.VolumeOff;
			}
			else if (presenter.Volume <= 33)
			{
				VolumeIcon.Kind = PackIconKind.VolumeLow;
			}
			else if (presenter.Volume <= 66)
			{
				VolumeIcon.Kind = PackIconKind.VolumeMedium;
			}
			else if (presenter.Volume <= 100)
			{
				VolumeIcon.Kind = PackIconKind.VolumeHigh;
			}
		}

		private void ToggleEnable(bool _state)
		{
			btnPrev.IsEnabled = _state;
			btnStop.IsEnabled = _state;
			btnNext.IsEnabled = _state;
			sliderTime.IsEnabled = _state;
			btnVolume.IsEnabled = _state;
			sliderVolume.IsEnabled = _state;
		}
	}
}

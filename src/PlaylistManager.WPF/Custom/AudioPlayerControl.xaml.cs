using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using PlaylistManager.BL;
using static PlaylistManager.BL.Tools;

namespace PlaylistManager.WPF.Custom
{
	/// <summary>
	///     Interaction logic for AudioPlayerControl.xaml
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{
		private readonly Manager _manager;

		public AudioPlayerControl()
		{
			InitializeComponent();
			_manager = new Manager();
			ToggleButtons();
			RegisterEventHandlers();
		}

		private void RegisterEventHandlers()
		{
			ButtonPrevious.Click += ButtonPrevious_Click;
			ButtonPrevious.IsEnabledChanged += ButtonPrevious_IsEnabledChanged;
			ButtonPlay.Click += ButtonPlay_Click;
			ButtonStop.Click += ButtonStop_Click;
			ButtonStop.IsEnabledChanged += ButtonStop_IsEnabledChanged;
			ButtonNext.Click += ButtonNext_Click;
			ButtonNext.IsEnabledChanged += ButtonNext_IsEnabledChanged;

			SongSlider.ValueChanged += SongSlider_ValueChanged;
			SongSlider.PreviewMouseDown += SongSlider_PreviewMouseDown;
			SongSlider.PreviewMouseUp += SongSlider_PreviewMouseUp;

			ButtonRepeat.Click += ButtonRepeat_Click;
			ButtonRepeat.IsEnabledChanged += ButtonRepeat_IsEnabledChanged;
			ButtonShuffle.Click += ButtonShuffle_Click;
			ButtonShuffle.IsEnabledChanged += ButtonShuffle_IsEnabledChanged;


			VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;
			VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
			SpeakerIcon.Click += SpeakerIcon_Click;
		}

		/// <summary>
		///     Navigation events:
		///     * Previous: go to previous song
		///     * Play/pause: toggle between pause and play
		///     * Next: go to next song
		/// </summary>
		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Prev button clicked!");
			_manager.Prev();
		}
		private void ButtonPrevious_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (ButtonPrevious.IsEnabled)
			{
				ButtonPrevious.Content = FindResource("Prev");
			}
			else
			{
				ButtonPrevious.Content = FindResource("PrevDisabled");
			}
		}
		private void ButtonPlay_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Play button clicked!");

			if (_manager.SongPlaying != null)
			{
				ButtonPlay.Content = FindResource("Play");
				_manager.ToggleResumePause();
			}
			else
			{
				_manager.LoadSong();

				var formattedDuration = FormatDuration(_manager.SongPlaying.Duration);
				LabelEndTime.Content = formattedDuration;

				if (formattedDuration.Length > 5)
				{
					LabelCurrentTime.Content = "00:00:00";
					GridContainer.ColumnDefinitions[4].Width = new GridLength(60);
					GridContainer.ColumnDefinitions[6].Width = new GridLength(60);
				}
				else
				{
					LabelCurrentTime.Content = "00:00";
					GridContainer.ColumnDefinitions[4].Width = new GridLength(40);
					GridContainer.ColumnDefinitions[6].Width = new GridLength(40);
				}

				ToggleButtons();
				ButtonPlay.Content = FindResource("Pause");

				LabelTitle.Content = _manager.SongPlaying.Title;
				LabelArtist.Content = _manager.SongPlaying.Artist;
				LabelAlbum.Content = _manager.SongPlaying.Album;

				_manager.Play();
				

				var b = new Binding();
				b.Source = _manager;
				b.Path = new PropertyPath("CurrentTime", _manager);
				b.Mode = BindingMode.OneWay;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				SongSlider.SetBinding(RangeBase.ValueProperty, b);

				SongSlider.Minimum = 0;
				SongSlider.Maximum = _manager.GetLengthInSeconds();
			}
		}
		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			_manager.Stop();
			ButtonPlay.Content = FindResource("Play");
			SongSlider.Value = 0;
			LabelEndTime.Content = "00:00";
			LabelTitle.Content = string.Empty;
			LabelArtist.Content = string.Empty;
			LabelAlbum.Content = string.Empty;
			ToggleButtons();
		}
		private void ButtonStop_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (ButtonStop.IsEnabled)
			{
				ButtonStop.Content = FindResource("Stop");
			}
			else
			{
				ButtonStop.Content = FindResource("StopDisabled");
			}
		}
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Next button clicked!");
			_manager.Next();
		}
		private void ButtonNext_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (ButtonNext.IsEnabled)
			{
				ButtonNext.Content = FindResource("Next");
			}
			else
			{
				ButtonNext.Content = FindResource("NextDisabled");
			}
		}

		/// <summary>
		///     SongSlider events:
		///     * ValueChanged: update current time label
		///     * PreviewMouseDown: indicates user want to use slider -> pause current song
		///     * PreviewMouseUp: indicates user changed the slider -> navigate to correct position in song
		/// </summary>
		private void SongSlider_ValueChanged(object sender, RoutedEventArgs e)
		{
			LabelCurrentTime.Content = FormatDuration(TimeSpan.FromSeconds(SongSlider.Value));
		}
		private void SongSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			_manager.Pause();
		}
		private void SongSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			_manager.SetPosition(SongSlider.Value);

			if (_manager.State == PlayState.Playing)
				_manager.Resume();
		}

		/// <summary>
		///     Songlist events:
		///     * Repeat with 3 states: no-repeat, repeat this song and repeat list
		///     * Shuffle with 2 states: shuffle-on, shuffle-off
		/// </summary>
		private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Repeat button clicked!");
			_manager.ToggleRepeat();

			switch (_manager.RepeatMode)
			{
				case RepeatMode.On:
					ButtonRepeat.Content = FindResource("RepeatOn");
					break;
				case RepeatMode.Once:
					ButtonRepeat.Content = FindResource("RepeatOnce");
					break;
				case RepeatMode.Off:
					ButtonRepeat.Content = FindResource("RepeatOff");
					break;
			}
		}
		private void ButtonRepeat_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (ButtonRepeat.IsEnabled)
			{
				switch (_manager.RepeatMode)
				{
					case RepeatMode.On:
						ButtonRepeat.Content = FindResource("RepeatOn");
						break;
					case RepeatMode.Off:
						ButtonRepeat.Content = FindResource("RepeatOff");
						break;
					case RepeatMode.Once:
						ButtonRepeat.Content = FindResource("RepeatOnce");
						break;
				}
			}
			else
			{
				ButtonRepeat.Content = FindResource("RepeatOff");
			}
		}
		private void ButtonShuffle_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Shuffle button clicked!");

			_manager.ShuffleEnabled = !_manager.ShuffleEnabled;

			if (_manager.ShuffleEnabled)
			{
				ButtonShuffle.Content = FindResource("ShuffleOn");
			}
			else
			{
				ButtonShuffle.Content = FindResource("ShuffleOff");
			}
		}
		private void ButtonShuffle_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (ButtonShuffle.IsEnabled)
			{
				if (_manager.ShuffleEnabled)
				{
					ButtonShuffle.Content = FindResource("ShuffleOn");
				}
				else
				{
					ButtonShuffle.Content = FindResource("ShuffleOff");
				}
			}
			else
			{
				ButtonShuffle.Content = FindResource("ShuffleOff");
			}
		}

		/// <summary>
		///     Volumeslider events:
		///     * PreviewMouseWheel: change volume slider with mousewheel when user hovers the control
		///     * ValueChanged: change volume when user drags volume control
		///     * SpeakerIcon: mute button and also speaker indicator
		/// </summary>
		private void VolumeSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			VolumeSlider.Value += VolumeSlider.SmallChange * e.Delta / 60;

			if (_manager.SongPlaying != null)
				_manager.SetVolume(VolumeSlider.Value / 100);
			Debug.WriteLine("Volume changed!");
		}
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_manager.SongPlaying != null)
				_manager.SetVolume(VolumeSlider.Value);
			Debug.WriteLine("Volume changed!");
		}
		private void SpeakerIcon_Click(object sender, RoutedEventArgs e)
		{
			VolumeSlider.Value = _manager.Mute();
			Debug.WriteLine("(Un)Muted!");
		}

		private void ToggleButtons()
		{
			ButtonPrevious.IsEnabled = !ButtonPrevious.IsEnabled;
			ButtonNext.IsEnabled = !ButtonNext.IsEnabled;
			ButtonStop.IsEnabled = !ButtonStop.IsEnabled;
			SongSlider.IsEnabled = !SongSlider.IsEnabled;
			ButtonRepeat.IsEnabled = !ButtonRepeat.IsEnabled;
			ButtonShuffle.IsEnabled = !ButtonShuffle.IsEnabled;
		}
	}
}
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using PlaylistManager.BL;
using PlaylistManager.Domain;
using static PlaylistManager.BL.Tools;

namespace PlaylistManager.WPF.Custom
{
	/// <summary>
	///     Interaction logic for AudioPlayerControl.xaml
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{
		public Manager Manager { get; set; }

		public AudioPlayerControl()
		{
			InitializeComponent();
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
		#region NavigationEvents
		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Prev button clicked!");
			Manager.Prev();
		}

		private void ButtonPrevious_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ButtonPrevious.Content = FindResource(ButtonPrevious.IsEnabled ? "Prev" : "PrevDisabled");
		}

		private void ButtonPlay_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Play button clicked!");

			if (Manager.CurrentSong != null)
			{
				ButtonPlay.Content = FindResource("Play");
				Manager.ToggleResumePause();
			}
			else if (Manager.SelectedSong != null)
			{
				Manager.CurrentSong = Manager.SelectedSong;
				StartPlaying(Manager.SelectedSong);
			}
			else
			{
				StartPlaying(null);
			}
		}

		private void ButtonStop_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Stop button clicked");

			Manager.Stop();
			ResetPlayer();
		}

		private void ButtonStop_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ButtonStop.Content = FindResource(ButtonStop.IsEnabled ? "Stop" : "StopDisabled");
		}

		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Next button clicked!");
			Manager.Next();
		}

		private void ButtonNext_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ButtonNext.Content = FindResource(ButtonNext.IsEnabled ? "Next" : "NextDisabled");
		}
		#endregion

		/// <summary>
		///     SongSlider events:
		///     * ValueChanged: update current time label
		///     * PreviewMouseDown: indicates user want to use slider -> pause current song
		///     * PreviewMouseUp: indicates user changed the slider -> navigate to correct position in song
		/// </summary>
		#region SongsliderEvents
		private void SongSlider_ValueChanged(object sender, RoutedEventArgs e)
		{
			LabelCurrentTime.Content = FormatDuration(TimeSpan.FromSeconds(SongSlider.Value));

			if (Manager.CurrentSongIsFinished())
			{
				Manager.Next();
			}
		}

		private void SongSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			Manager.Pause();
		}

		private void SongSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			Manager.SetPosition(SongSlider.Value);

			if (Manager.State == PlayState.Playing)
				Manager.Resume();
		}
		#endregion

		/// <summary>
		///     Songlist events:
		///     * Repeat with 3 states: no-repeat, repeat this song and repeat list
		///     * Shuffle with 2 states: shuffle-on, shuffle-off
		/// </summary>
		#region SonglistEvents
		private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Repeat button clicked!");
			Manager.ToggleRepeat();

			switch (Manager.RepeatMode)
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
				switch (Manager.RepeatMode)
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

			Manager.ToggleShuffle();
			ButtonShuffle.Content = FindResource(Manager.ShuffleEnabled ? "ShuffleOn" : "ShuffleOff");
		}

		private void ButtonShuffle_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			Debug.WriteLine("Shuffle changed state");

			if (ButtonShuffle.IsEnabled)
			{
				ButtonShuffle.Content = FindResource(Manager.ShuffleEnabled ? "ShuffleOn" : "ShuffleOff");
			}
			else
			{
				ButtonShuffle.Content = FindResource("ShuffleOff");
			}
		}
		#endregion

		/// <summary>
		///     Volumeslider events:
		///     * PreviewMouseWheel: change volume slider with mousewheel when user hovers the control
		///     * ValueChanged: change volume when user drags volume control
		///     * SpeakerIcon: mute button and also speaker indicator
		/// </summary>
		#region Volumeslider events
		private void VolumeSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			VolumeSlider.Value += VolumeSlider.SmallChange * e.Delta / 60;
		}

		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			var value = VolumeSlider.Value;

			if (Manager.CurrentSong != null)
				Manager.SetVolume(value, songPlaying: true);
			else
				Manager.SetVolume(value, songPlaying: false);

			Debug.WriteLine(VolumeSlider.Value);

			uint iValue = (uint) value;

			if (iValue == 0)
			{
				SpeakerIcon.Content = FindResource("SpeakerOff");
			}
			else if (iValue <= 33)
			{
				SpeakerIcon.Content = FindResource("Speaker33");
			}
			else if (iValue <= 66)
			{
				SpeakerIcon.Content = FindResource("Speaker66");
			}
			else if (iValue <= 100)
			{
				SpeakerIcon.Content = FindResource("Speaker100");
			}

			Debug.WriteLine("Volume changed!");
		}

		private void SpeakerIcon_Click(object sender, RoutedEventArgs e)
		{
			VolumeSlider.Value = Manager.Mute();
			Debug.WriteLine("(Un)Muted!");
		}
		#endregion

		#region auxilary methods
		internal void ToggleButtons()
		{
			switch (Manager.State)
			{
				case PlayState.Playing:
					ButtonPrevious.IsEnabled = Manager.HasPrev();
					ButtonNext.IsEnabled = Manager.HasNext();
					ButtonPlay.IsEnabled = true;
					ButtonPlay.Content = FindResource("Pause");
					SongSlider.IsEnabled = true;
					ButtonRepeat.IsEnabled = true;
					ButtonShuffle.IsEnabled = true;
					break;

				case PlayState.Paused:
					ButtonPrevious.IsEnabled = false;
					ButtonPlay.IsEnabled = true;
					ButtonPlay.Content = FindResource("Play");
					ButtonNext.IsEnabled = false;
					break;

				case PlayState.Stopped:
					ButtonPrevious.IsEnabled = false;
					ButtonPlay.IsEnabled = true;
					ButtonPlay.Content = FindResource("Play");
					ButtonNext.IsEnabled = false;
					ButtonStop.IsEnabled = false;
					SongSlider.IsEnabled = false;
					ButtonRepeat.IsEnabled = false;
					ButtonShuffle.IsEnabled = false;
					return;
			}

			ButtonStop.IsEnabled = true;
		}

		public void StartPlaying(Song song)
		{
			if (song == null)
			{
				song = Manager.GetRandomSong();
			}

			Manager.CurrentSong = song;
			ButtonPlay.Content = FindResource("Pause");

			Manager.SetVolume(VolumeSlider.Value, songPlaying: false);
			Manager.Play();
		}

		internal void RetrieveSongInfo()
		{
			if (Manager.CurrentSong == null) return;

			//Retrieve & format time
			var formattedDuration = FormatDuration(Manager.CurrentSong.Duration);
			LabelEndTime.Content = formattedDuration;
			LabelCurrentTime.Content = FormatDuration(TimeSpan.FromSeconds(Manager.GetPositionInSeconds()));

			if (formattedDuration.Length > 5)
			{
				GridContainer.ColumnDefinitions[4].Width = new GridLength(60);
				GridContainer.ColumnDefinitions[6].Width = new GridLength(60);
			}
			else
			{
				GridContainer.ColumnDefinitions[4].Width = new GridLength(40);
				GridContainer.ColumnDefinitions[6].Width = new GridLength(40);
			}

			SongSlider.Minimum = 0;
			SongSlider.Maximum = Manager.GetLengthInSeconds();

			var b = new Binding 
			{
				Source = Manager,
				Path = new PropertyPath("CurrentTime", Manager),
				Mode = BindingMode.OneWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			SongSlider.SetBinding(RangeBase.ValueProperty, b);

			//Retrieve & set other song information of song playing
			LabelTitle.Content = Manager.CurrentSong.Title;
			LabelArtist.Content = Manager.CurrentSong.Artist;
			LabelAlbum.Content = Manager.CurrentSong.Album;
		}

		private void ResetPlayer()
		{
			ButtonPlay.Content = FindResource("Play");
			SongSlider.Value = 0;
			LabelEndTime.Content = "00:00";
			LabelTitle.Content = string.Empty;
			LabelArtist.Content = string.Empty;
			LabelAlbum.Content = string.Empty;
		}

		#endregion
	}
}
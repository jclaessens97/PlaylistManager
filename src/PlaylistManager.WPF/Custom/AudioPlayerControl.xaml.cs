﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlaylistManager.BL;
using PlaylistManager.Domain;
using static PlaylistManager.BL.Tools;

namespace PlaylistManager.WPF.Custom
{
	/// <summary>
	/// Interaction logic for AudioPlayerControl.xaml
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{
		private readonly AudioManager _audioManager;

		public AudioPlayerControl()
		{
			InitializeComponent();
			_audioManager = new AudioManager();
			RegisterEventHandlers();

			//Slider values
			VolumeSlider.Value = 100;
		}

		private void RegisterEventHandlers()
		{
			ButtonPrevious.Click += ButtonPrevious_Click;
			ButtonPlay.Click += ButtonPlay_Click;
			ButtonNext.Click += ButtonNext_Click;

			SongSlider.ValueChanged += SongSlider_ValueChanged;
			SongSlider.PreviewMouseDown += SongSlider_PreviewMouseDown;
			SongSlider.PreviewMouseUp += SongSlider_PreviewMouseUp;
			
			ButtonRepeat.Click += ButtonRepeat_Click;
			ButtonShuffle.Click += ButtonShuffle_Click;

			VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;
			VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
			SpeakerIcon.Click += SpeakerIcon_Click;
		}

		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Prev button clicked!");
			_audioManager.Prev();
		}
		private void ButtonPlay_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Play button clicked!");

			if (_audioManager.SongPlaying != null)
			{
				_audioManager.ToggleResumePause();
			}
			else
			{
				_audioManager.LoadSong();

				string formattedDuration = FormatDuration(_audioManager.SongPlaying.Duration);
				LabelEndTime.Content = formattedDuration;

				if (formattedDuration.Length > 5)
				{
					LabelCurrentTime.Content = "00:00:00";
					GridContainer.ColumnDefinitions[3].Width = new GridLength(60);
					GridContainer.ColumnDefinitions[5].Width = new GridLength(60);
				}
				else
				{
					LabelCurrentTime.Content = "00:00";
					GridContainer.ColumnDefinitions[3].Width = new GridLength(40);
					GridContainer.ColumnDefinitions[5].Width = new GridLength(40);
				}

				_audioManager.Play();

				Binding b = new Binding();
				b.Source = _audioManager;
				b.Path = new PropertyPath("CurrentTime", _audioManager);
				b.Mode = BindingMode.OneWay;
				b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
				SongSlider.SetBinding(Slider.ValueProperty, b);

				SongSlider.Minimum = 0;
				SongSlider.Maximum = _audioManager.GetLengthInSeconds();
			}
		}
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Next button clicked!");
			_audioManager.Next();
		}

		private void SongSlider_ValueChanged(object sender, RoutedEventArgs e)
		{
			LabelCurrentTime.Content = FormatDuration(TimeSpan.FromSeconds(SongSlider.Value));
		}
		private void SongSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			_audioManager.Pause();
		}
		private void SongSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			_audioManager.SetPosition(SongSlider.Value);

			if (_audioManager.State == PlayState.Playing)
			{
				_audioManager.Resume();
			}
		}

		private void ButtonRepeat_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Repeat button clicked!");
		}
		private void ButtonShuffle_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Shuffle button clicked!");
		}

		private void VolumeSlider_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
		{
			VolumeSlider.Value += VolumeSlider.SmallChange * e.Delta / 60;

			if (_audioManager.SongPlaying != null)
			{
				_audioManager.SetVolume(VolumeSlider.Value / 100);
			}
			Debug.WriteLine("Volume changed!");
		}
		private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (_audioManager.SongPlaying != null)
			{
				_audioManager.SetVolume(VolumeSlider.Value);
			}
			Debug.WriteLine("Volume changed!");
		}
		private void SpeakerIcon_Click(object sender, RoutedEventArgs e)
		{
			VolumeSlider.Value = _audioManager.Mute();
			Debug.WriteLine("(Un)Muted!");
		}
	}
}

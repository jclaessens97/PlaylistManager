using System;
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

namespace PlaylistManager.WPF.Custom
{
	/// <summary>
	/// Interaction logic for AudioPlayerControl.xaml
	/// </summary>
	public partial class AudioPlayerControl : UserControl
	{
		private readonly AudioPlayer _audioPlayer = new AudioPlayer();

		public AudioPlayerControl()
		{
			InitializeComponent();
			RegisterEventHandlers();
		}

		private void RegisterEventHandlers()
		{
			ButtonPrevious.Click += ButtonPrevious_Click;
			ButtonPlay.Click += ButtonPlay_Click;
			ButtonNext.Click += ButtonNext_Click;

			//if (GetThumb(SongSlider) != null)
			//{
			//	GetThumb(SongSlider).DragStarted += SongSlider_DragStarted;
			//	GetThumb(SongSlider).DragCompleted += SongSlider_DragCompleted;
			//}

			ButtonRepeat.Click += ButtonRepeat_Click;
			ButtonShuffle.Click += ButtonShuffle_Click;

			VolumeSlider.PreviewMouseWheel += VolumeSlider_PreviewMouseWheel;
			SpeakerIcon.Click += SpeakerIcon_Click;
		}

		private Thumb GetThumb(Slider slider)
		{
			var track = slider.Template.FindName("PART_Track", slider) as Track;
			return track == null ? null : track.Thumb;
		}

		private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Prev button clicked!");
		}
		private void ButtonPlay_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Play button clicked!");
		}
		private void ButtonNext_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Next button clicked!");
		}

		//private void SongSlider_DragStarted(object sender, RoutedEventArgs e)
		//{
		//	Debug.WriteLine("Drag started!");
		//}
		//private void SongSlider_DragCompleted(object sender, RoutedEventArgs e)
		//{
		//	Debug.WriteLine("Drag stopped!");
		//}

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
			Debug.WriteLine("Scroll wheel used!");
			VolumeSlider.Value += VolumeSlider.SmallChange * e.Delta / 120;
		}
		private void SpeakerIcon_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("SpeakerIcon clicked!");
		}
	}
}

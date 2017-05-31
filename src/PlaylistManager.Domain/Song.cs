using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PlaylistManager.Domain.Annotations;
using TagLib;
using TagLib.Riff;

namespace PlaylistManager.Domain
{
	/// <summary>
	///     Model that holds all song properties
	///     (same datatypes as defined in the TagLib library)
	/// </summary>
	public class Song : INotifyPropertyChanged
	{
		private bool _isPlaying;
		
		public bool IsPlaying
		{
			get => _isPlaying;
			set
			{
				if (value.Equals(_isPlaying)) return;
				_isPlaying = value;
				OnPropertyChanged();
			}
		}

		public uint Id { get; set; }
		public string Title { get; set; }
		public string Album { get; set; }
		public TimeSpan Duration { get; set; }
		public string Path { get; set; }
		public string[] Genres { get; set; }
		public string Artist { get; set; }
		public uint? Year { get; set; }
		public uint? TrackNumber { get; set; }
		public IPicture AlbumArt { get; set; }

		public override string ToString()
		{
			return $"{Id}) - {Artist} - {Title} ({Duration:hh\\:mm\\:ss})";
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
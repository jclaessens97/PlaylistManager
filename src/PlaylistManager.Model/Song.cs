using System;
using PlaylistManager.Model.Other;
using TagLib;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Song (same datatypes as defined in the TagLib library)
	/// </summary>
	public class Song : ObservableObject
	{
		#region Attributes

		private string title;
		private string album;
		private TimeSpan duration;
		private string path;
		private string[] genres;
		private string artist;
		private uint? year;
		private uint? trackNumber;
		private IPicture albumArt;
		private bool isPlaying;

		#endregion

		#region Properties

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
		public TimeSpan Duration
		{
			get => duration;
			set
			{
				duration = value;
				RaisePropertyChangedEvent(nameof(Duration));
			}
		}
		public string Path
		{
			get => path;
			set
			{
				path = value;
				RaisePropertyChangedEvent(nameof(Path));
			}
		}
		public string[] Genres
		{
			get => genres;
			set
			{
				genres = value;
				RaisePropertyChangedEvent(nameof(Genres));
			}
		}
		public uint? Year
		{
			get => year;
			set
			{
				year = value;
				RaisePropertyChangedEvent(nameof(Year));
			}
		}
		public uint? TrackNumber
		{
			get => trackNumber;
			set
			{
				trackNumber = value;
				RaisePropertyChangedEvent(nameof(TrackNumber));
			}
		}
		public IPicture AlbumArt
		{
			get => albumArt;
			set
			{
				albumArt = value;
				RaisePropertyChangedEvent(nameof(AlbumArt));
			}
		}
		public bool IsPlaying
		{
			get => isPlaying;
			set
			{
				isPlaying = value;
				RaisePropertyChangedEvent(nameof(IsPlaying));
			}
		}

		#endregion

		public override string ToString()
		{
			return $"{Artist} - {Title} ({Duration:hh\\:mm\\:ss})";
		}
	}
}
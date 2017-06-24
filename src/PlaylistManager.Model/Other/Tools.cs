using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace PlaylistManager.Model.Other
{
	/// <summary>
	///     Static toolbox with methods I need troughout the project and I don't
	///     know any better place than a seperate static class
	/// </summary>
	public static class Tools
	{
		public static string FormatDuration(TimeSpan _duration)
		{
			var durationStr = _duration.ToString();

			if (durationStr.StartsWith("00:"))
			{
				return durationStr.Substring(3, 5);
			}
			else
			{
				return durationStr.Substring(0, 8);
			}
		}

		public static string FormatDuration(double _duration)
		{
			TimeSpan ts = TimeSpan.FromSeconds(_duration);
			return FormatDuration(ts);
		}

		public static void Sort(List<Song> _sourceCollection, string _member, ListSortDirection? _sortDirection)
		{
			int direction = (_sortDirection == ListSortDirection.Ascending ? 1 : -1);

			switch (_member)
			{
				case nameof(Song.Title):
					_sourceCollection.Sort((s1, s2) => s1.Title.CompareTo(s2.Title) * direction);
					break;
				case nameof(Song.Artist):
					_sourceCollection.Sort((s1, s2) => s1.Artist.CompareTo(s2.Artist) * direction);
					break;
				case nameof(Song.Duration):
					_sourceCollection.Sort((s1, s2) => s1.Duration.CompareTo(s2.Duration) * direction);
					break;
				case nameof(Song.Album):
					_sourceCollection.Sort((s1, s2) => s1.Album.CompareTo(s2.Album) * direction);
					break;
				//				case nameof(Song.Genres):
				//					sourceCollection.Sort((s1, s2) => s1.Duration.CompareTo(s2.Duration) * direction);
				//					break;
				case nameof(Song.Year):
					if (direction == 1)
						_sourceCollection = _sourceCollection.OrderBy(s => s.Year).ThenBy(s => s.Id).ToList();
					else
						_sourceCollection = _sourceCollection.OrderByDescending(s => s.Year).ThenByDescending(s => s.Id).ToList();
					break;

			}
		}
	}
}
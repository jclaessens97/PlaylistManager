using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using PlaylistManager.Domain;

namespace PlaylistManager.BL
{
	/// <summary>
	///     Static toolbox with methods I need troughout the project and I don't
	///     know any better place than a seperate static class
	/// </summary>
	public static class Tools
	{
		public static string FormatDuration(TimeSpan duration)
		{
			var durationStr = duration.ToString();

			if (durationStr.StartsWith("00:"))
			{
				return durationStr.Substring(3, 5);
			}
			else
			{
				return durationStr.Substring(0, 8);
			}
		}

		public static void Sort(List<Song> sourceCollection, string member, ListSortDirection? sortDirection)
		{
			int direction = (sortDirection == ListSortDirection.Ascending ? 1 : -1);

			switch (member)
			{
				case nameof(Song.Title):
					sourceCollection.Sort((s1, s2) => s1.Title.CompareTo(s2.Title) * direction);
					break;
				case nameof(Song.Artist):
					sourceCollection.Sort((s1, s2) => s1.Artist.CompareTo(s2.Artist) * direction);
					break;
				case nameof(Song.Duration):
					sourceCollection.Sort((s1, s2) => s1.Duration.CompareTo(s2.Duration) * direction);
					break;
				case nameof(Song.Album):
					sourceCollection.Sort((s1, s2) => s1.Album.CompareTo(s2.Album) * direction);
					break;
				//				case nameof(Song.Genres):
				//					sourceCollection.Sort((s1, s2) => s1.Duration.CompareTo(s2.Duration) * direction);
				//					break;
				case nameof(Song.Year):
					sourceCollection.Sort((s1, s2) => s1.Year.CompareTo(s2.Year) * direction);
					break;

			}
		}
	}
}
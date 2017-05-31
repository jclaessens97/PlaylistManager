using System;
using System.Collections.Generic;
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

		public static string GetPropertyName<T>(Expression<Func<T>> expression)
		{
			return (expression.Body as MemberExpression).Member.Name;
		}

		public static void Sort(List<Song> sourceCollection, string member, int direction)
		{
			switch (member)
			{
				default:
					sourceCollection.Sort((s1, s2) => s1.Id.CompareTo(s2.Id) * direction);
					break;
				case nameof(Song.Title):
					sourceCollection.Sort((s1, s2) => s1.Title.CompareTo(s2.Title) * direction);
					break;
			}
		}
	}
}
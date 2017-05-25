using System;
using System.Linq.Expressions;

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
	}
}
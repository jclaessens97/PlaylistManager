using System;


namespace PlaylistManager.Model.Other
{
	/// <summary>
	///     Static toolbox with methods I need troughout the project and I don't
	///     know any better place than a seperate static class
	/// </summary>
	public static class Tools
	{
		private static string FormatDuration(TimeSpan _duration)
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
	}
}
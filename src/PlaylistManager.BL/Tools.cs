using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.BL
{
	public static class Tools
	{
		public static string FormatDuration(TimeSpan duration)
		{
			string durationStr = duration.ToString();

			if (durationStr.StartsWith("00:"))
			{
				return durationStr.Substring(3, 5);
			}
			else
			{
				return durationStr.Substring(0, 8);
			}
		}

	}
}

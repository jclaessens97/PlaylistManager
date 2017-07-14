using System;


namespace PlaylistManager.Model.Other
{
	/// <summary>
	///     Static toolbox with methods I need troughout the project and I don't
	///     know any better place than a seperate static class
	/// </summary>
	public static class Tools
	{
        #region Formatting

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

	    #endregion

	    public static int GenerateHashCode(TagLib.File _file)
	    {
	        var title = _file.Tag.Title;
	        var artist = _file.Tag.Performers[0];
	        var duration = _file.Properties.Duration;

            return (title.GetHashCode() * 7) + (artist.GetHashCode() * 11) + (duration.GetHashCode() * 13) * 397;
	    }
    }
}
using System;


namespace PlaylistManager.Model.Other
{
	/// <summary>
	/// Class with global constants
	/// </summary>
	public static class Globals
	{
        #region Debug

#if DEBUG

	    public const string DEBUG_SONG_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\Ed Sheeran - Trap Queen (cover).mp3";
	    public const string DEBUG_SONG_PATH_LONG = @"D:\Bibliotheken\Muziek\Mijn muziek\Chill Mix 2015 (Eric Clapton).mp3"; //>1h
	    public const string DEBUG_MUSIC_FOLDER_PATH = @"D:\Bibliotheken\Muziek\Mijn muziek\";
	    //		public const string DEBUG_MUSIC_FOLDER_PATH = @"D:\Bibliotheken\Downloads\Muziek\";
	    public const bool DEBUG_INCLUDE_SUBDIRS = true;
	    public static readonly Song DEBUG_SONG = new Song()
	    {
	        Title = "DebugSong",
	        Album = "DebugAlbum",
	        Artist = "DebugArtist",
	        Duration = TimeSpan.FromSeconds(215), //3:35 min in seconds
	        Path = DEBUG_SONG_PATH
	    };

#endif

        #endregion

	    #region Settings

	    public const string FolderKey = "Folder";
	    public const string SubdirsKey = "SubDirs";
	    public const string TimeBetweenSongsKey = "TimeBetweenSongs";

	    #endregion
	}
}

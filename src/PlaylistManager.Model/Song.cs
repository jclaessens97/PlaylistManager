using System;
using System.ComponentModel;
using PlaylistManager.Model.Other;
using TagLib;

namespace PlaylistManager.Model
{
	/// <summary>
	/// Song (same datatypes as defined in the TagLib library)
	/// </summary>
	public class Song : INotifyPropertyChanged
	{
        #region Attributes

	    private bool isPlaying;

        #endregion

        #region Properties

        public string Title { get; set; }
	    public string Artist { get; set; }
	    public string Album { get; set; }
	    public TimeSpan Duration { get; set; }
	    public string Path { get; set; }
	    public string[] Genres { get; set; }
	    public uint? Year { get; set; }
	    public uint? TrackNumber { get; set; }
        public IPicture AlbumArt { get; set; }
        public bool IsPlaying
	    {
	        get => isPlaying;
	        set
	        {
	            isPlaying = value;
	            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsPlaying)));
	        }
	    }

        #endregion

        #region Events

	    public event PropertyChangedEventHandler PropertyChanged;

	    private void OnPropertyChanged(PropertyChangedEventArgs _e)
	    {
	        PropertyChangedEventHandler handler = PropertyChanged;
            handler?.Invoke(this, _e);
	    }

        #endregion

        public override string ToString()
		{
			return $"{Artist} - {Title} ({Duration:hh\\:mm\\:ss})";
		}

	}
}
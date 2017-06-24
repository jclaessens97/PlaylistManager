using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.Model;

namespace PlaylistManager.ViewModel
{
	public class LibraryPresenter : ObservableObject
	{
		#region Attributes

		private readonly Library library;

		#endregion

		#region Properties

		public ObservableCollection<SongViewModel> SongsInLibrary { get; private set; }

		#endregion

		public LibraryPresenter()
		{	
			library = new Library();

			SongsInLibrary = LoadSongs();
		}

		private ObservableCollection<SongViewModel> LoadSongs()
		{
			var songsInLibrary = new ObservableCollection<SongViewModel>();

			foreach (var s in library.Songs)
			{
				var song = new SongViewModel()
				{
					Title = s.Title,
					Artist = s.Artist,
					Album = s.Album,
					Duration = s.Duration,
					Path = s.Path,
					Genres = s.Genres,
					Year = s.Year,
					TrackNumber = s.TrackNumber
				};

				songsInLibrary.Add(song);
			}

			return songsInLibrary;
		}
	}
}

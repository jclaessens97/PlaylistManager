using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Other;

namespace PlaylistManager.ViewModel.Presenters
{
	public class LibraryPresenter : ObservableObject
	{
		#region Attributes

		private readonly Library library;

		private ObservableCollection<Song> songsInLibrary;

		private Song selectedSong;
		private List<Song> selectedSongs;

		private ICommand doubleClickRowCommand;

		#endregion

		#region Properties

		public AudioplayerPresenter AudioplayerPresenter { get; set; }

		public ObservableCollection<Song> SongsInLibrary
		{
			get => songsInLibrary;
			set
			{
				songsInLibrary = value;
				library.Songs = songsInLibrary.ToList();
			}
		}

		#endregion

		#region Commands

		public ICommand DoubleClickRowCommand
		{
			get
			{
				doubleClickRowCommand = new RelayCommand(
					item =>
					{
						PlaySelectedSong(item as Song);
					}
				);

				return doubleClickRowCommand;
			}
		}

		#endregion


		public LibraryPresenter()
		{	
			library = Library.Instance;

			LoadSongs();
		}

		#region LibraryGrid Events

		private void PlaySelectedSong(Song _selectedSong)
		{
			AudioplayerPresenter.Start(_selectedSong);
		}

		#endregion

		#region Auxilary

		private void LoadSongs()
		{
			SongsInLibrary = new ObservableCollection<Song>(library.Songs);
		}

		private void GeneratePlayingNowList()
		{
			
		}

		#endregion
	}
}

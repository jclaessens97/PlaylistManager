using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.Other;

namespace PlaylistManager.ViewModel.ViewModels
{
    public sealed class LibraryControlViewModel : ObservableObject
    {
        #region Attributes
        
        private ObservableCollection<Song> songsInLibrary;
        private ICollectionView librarySongsource;

        #endregion

        #region Properties

        public ILibraryControl LibraryControl { get; set; }
        public Library Library { get; }

        public ObservableCollection<Song> SongsInLibrary
        {
            get => songsInLibrary;
            set
            {
                songsInLibrary = value;
                Library.Songs = songsInLibrary.ToList();
            }
        }

        #endregion

        #region Commands

        public ICommand SortedCommand => new DelegateCommand(OnSorted);
        public ICommand DoubleClickRowCommand
        {
            get
            {
                var doubleClickRowCommand = new RelayCommand(item => Messenger.Instance.Send(item as Song, MessageContext.PlayOneSongDoubleClick));
                return doubleClickRowCommand;
            }
        }

        #endregion

        #region Events

        private void OnSorted()
        {
            var sortedList = new ObservableCollection<Song>();
            foreach (var song in librarySongsource.OfType<Song>())
            {
                sortedList.Add(song);  
            }

            SongsInLibrary = sortedList;
        }

        #endregion

        public LibraryControlViewModel()
        {
            Library = Library.Instance;
        }

        #region Load
        
        /// <summary>
        /// Fill library grid after converting to collectionviewsource
        /// </summary>
        public void FillLibraryGrid()
        {
            LoadSongs();
            librarySongsource = CollectionViewSource.GetDefaultView(SongsInLibrary);
            LibraryControl.LibraryDataGrid.ItemsSource = librarySongsource;
        }

        /// <summary>
        /// Load songs from model to observable collection
        /// </summary>
        private void LoadSongs()
        {
            SongsInLibrary = new ObservableCollection<Song>(Library.Songs);
        }

        /// <summary>
        /// Reload songs (after change in settings)
        /// </summary>
        public void ReloadSongs()
        {
            Library.LoadSongs();
            this.LoadSongs();
        }

        #endregion
    }
}

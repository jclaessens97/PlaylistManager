using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PlaylistManager.Model;
using PlaylistManager.ViewModel.Presenters;

namespace PlaylistManager.View.UserControls
{
    /// <summary>
    /// Interaction logic for LibraryControl.xaml
    /// </summary>
    public partial class LibraryControl : UserControl
    {
	    private LibraryPresenter libraryPresenter;
		private ICollectionView librarySongSource;

		public LibraryControl()
        {
            InitializeComponent();
		}

	    public void LoadLibrary()
	    {
			if (DataContext != null)
		    {
			    libraryPresenter = DataContext as LibraryPresenter;
		    }

		    if (libraryPresenter != null)
		    {
			    librarySongSource = CollectionViewSource.GetDefaultView(libraryPresenter.SongsInLibrary);
			    libraryDataGrid.ItemsSource = librarySongSource;
		    }
		}

	    private void LibraryDataGrid_OnSorted(object _sender, RoutedEventArgs _e)
	    {
		    var sortedList = new ObservableCollection<Song>();
		    foreach (var song in librarySongSource.OfType<Song>())
		    {
			    //Debug.WriteLine(song);
			    sortedList.Add(song);
		    }

		    libraryPresenter.SongsInLibrary = sortedList;
		    //libraryPresenter.SongsInLibrary.ToList().ForEach(s => Debug.WriteLine(s));
	    }
	}
}

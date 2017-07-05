using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PlaylistManager.Model;
using PlaylistManager.ViewModel.Presenters;

namespace PlaylistManager.View.UserControls
{
    /// <summary>
    /// Interaction logic for LibraryControl.xaml
    /// </summary>
    public partial class LibraryControl : UserControl
    {
		#region Attributes

	    private LibraryPresenter libraryPresenter;
	    private ICollectionView librarySongSource;

		#endregion

		public LibraryControl()
        {
            InitializeComponent();
		}

		/// <summary>
		/// Fills grid with songs from library
		/// Called after datacontext is set to librarypresenter
		/// </summary>
	    public void LoadLibrary()
	    {
		    if (DataContext is LibraryPresenter libraryPresenter)
		    {
			    this.libraryPresenter = libraryPresenter;
				librarySongSource = CollectionViewSource.GetDefaultView(libraryPresenter.SongsInLibrary);
			    libraryDataGrid.ItemsSource = librarySongSource;
			}
		}

		/// <summary>
		/// Method from CustomDataGrid.cs
		/// Event fired after sorting is complete
		/// </summary>
		/// <param name="_sender"></param>
		/// <param name="_e"></param>
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

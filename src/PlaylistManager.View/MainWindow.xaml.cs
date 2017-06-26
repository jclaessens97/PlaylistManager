using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using PlaylistManager.ViewModel;
using PlaylistManager.ViewModel.Presenters;

namespace PlaylistManager.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly AudioplayerPresenter audioplayerPresenter;
		private readonly LibraryPresenter libraryPresenter;
		private readonly ICollectionView librarySongSource;

		public MainWindow()
		{
			InitializeComponent();

			//Set the presenter of the Audioplayer control
			audioplayerPresenter = new AudioplayerPresenter();
			AudioPlayerControl.DataContext = audioplayerPresenter;
			AudioPlayerControl.RegisterEvents();

			//Load the data context & itemssource of the library grid programmatically via converting it to a ICollectionView
			libraryPresenter = new LibraryPresenter();
			DataContext = libraryPresenter;
			librarySongSource = CollectionViewSource.GetDefaultView(libraryPresenter.SongsInLibrary);

			libraryDataGrid.ItemsSource = librarySongSource;

			//Set a reference in libraryPresenter to AudioplayerPresenter so you can access its methods (e.g. to start a song)
			libraryPresenter.AudioplayerPresenter = audioplayerPresenter;
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

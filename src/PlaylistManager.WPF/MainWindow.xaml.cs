using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using PlaylistManager.BL;
using PlaylistManager.Domain;

namespace PlaylistManager.WPF
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Manager _manager;

		public MainWindow()
		{
			InitializeComponent();
			Title = "PlaylistManager v0.1";
			WindowState = WindowState.Maximized;

			_manager = new Manager();
			AudioPlayerControl.Manager = _manager;

			LoadLibrary();
			AddEventHandlers();
		}

		private void LoadLibrary()
		{
			if (_manager.CheckLibrary())
			{
				foreach (var song in _manager.GetLibrarySongs())
				{
					LibraryView.Items.Add(song);
				}
			}
			else
			{
				//TODO: if library empty
			}
		}

		private void AddEventHandlers()
		{
			LibraryView.Loaded += LibraryView_Loaded;
			LibraryView.Sorting += LibraryView_Sorting;
			LibraryView.SelectedCellsChanged += LibraryView_SelectedCellsChanged;

			//Datagrid events
			Style rowStyle = new Style(typeof(DataGridRow));
			rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, 
												new MouseButtonEventHandler(LibraryViewRow_DoubleClick)));
			LibraryView.RowStyle = rowStyle;
		}

		private void LibraryView_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var col in LibraryView.Columns)
			{
				col.Width = new DataGridLength(col.ActualWidth, DataGridLengthUnitType.Pixel);
			}
		}

		private void LibraryView_Sorting(object sender, DataGridSortingEventArgs e)
		{
			Debug.WriteLine($"BEFORE: Sorting {e.Column.SortMemberPath} by {e.Column.SortDirection}");

			var sortDirection = e.Column.SortDirection;
			switch (sortDirection)
			{
				default: 
				case ListSortDirection.Descending:	//not needed, but makes things clearer
					sortDirection = ListSortDirection.Ascending;
					break;
				case ListSortDirection.Ascending:
					sortDirection = ListSortDirection.Descending;
					break;
			}

			_manager.SortLibrary(e.Column.SortMemberPath, sortDirection);
			LibraryView.Items.Clear();
			LoadLibrary();

			Debug.WriteLine($"AFTER: Sorting {e.Column.SortMemberPath} by {sortDirection}");
		}

		private void LibraryView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			DataGrid grid = sender as DataGrid;
			Debug.Assert(grid != null);

			if (grid.SelectedItems.Count == 1 && _manager.State == PlayState.Stopped)
			{
				_manager.CurrentSong = null;
				_manager.SelectedSong = grid.SelectedItem as Song;
				Debug.WriteLine("Selected song is:" + _manager.CurrentSong);
			}
			else if (grid.SelectedItems.Count > 1 && _manager.State == PlayState.Stopped)
			{
				_manager.CurrentSong = null;
				_manager.SelectedSong = grid.SelectedItems[0] as Song;

				List<Song> songs = new List<Song>(grid.SelectedItems.Count);
				songs.AddRange(from object song in grid.SelectedItems select song as Song);
				_manager.SetNowPlayingList(songs);

				_manager.PrintNowPlayingList();
			}
		}

		private void LibraryViewRow_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			if (row != null)
			{
				Song song = row.Item as Song;

				if (_manager.CurrentSong != null)
				{
					_manager.Stop();
				}

				AudioPlayerControl.StartPlaying(song);
			}
		}
	}
}
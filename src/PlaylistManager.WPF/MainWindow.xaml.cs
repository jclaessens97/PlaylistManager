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
		private readonly Manager manager;

		public MainWindow()
		{
			InitializeComponent();
			Title = "PlaylistManager v0.1";
			WindowState = WindowState.Maximized;

			manager = new Manager();
			AudioPlayerControl.Manager = manager;

			LoadLibrary();
			AddEventHandlers();
		}

		private void LoadLibrary()
		{
			if (manager.CheckLibrary())
			{
				foreach (var song in manager.GetLibrarySongs())
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
			//TODO

			Debug.WriteLine($"Sorting {e.Column.SortMemberPath} by {e.Column.SortDirection}");

			//LibraryView.ColumnFromDisplayIndex(e.Column.DisplayIndex);

			var sortDirection = e.Column.SortDirection;

			if (sortDirection == ListSortDirection.Descending || sortDirection == null)
			{
				e.Column.SortDirection = ListSortDirection.Ascending;
			}
			else
			{
				e.Column.SortDirection = ListSortDirection.Descending;
			}

			manager.SortLibrary(e.Column.SortMemberPath, sortDirection);
		}

		private void LibraryView_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			DataGrid grid = sender as DataGrid;
			Debug.Assert(grid != null);

			if (grid.SelectedItems.Count == 1 && manager.State == PlayState.Stopped)
			{
				manager.CurrentSong = null;
				manager.SelectedSong = grid.SelectedItem as Song;
				Debug.WriteLine("Selected song is:" + manager.CurrentSong);
			}
			else if (grid.SelectedItems.Count > 1 && manager.State == PlayState.Stopped)
			{
				manager.CurrentSong = null;
				manager.SelectedSong = grid.SelectedItems[0] as Song;

				List<Song> songs = new List<Song>(grid.SelectedItems.Count);
				songs.AddRange(from object song in grid.SelectedItems select song as Song);
				manager.SetNowPlayingList(songs);

				manager.PrintNowPlayingList();
			}
		}

		private void LibraryViewRow_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			if (row != null)
			{
				Song song = row.Item as Song;

				if (manager.CurrentSong != null)
				{
					manager.Stop();
				}

				AudioPlayerControl.StartPlaying(song);
			}
		}
	}
}
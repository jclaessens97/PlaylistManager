using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;

using PlaylistManager.BL;
using PlaylistManager.Domain;
using DataGrid = System.Windows.Controls.DataGrid;

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
			StartTimer();
			AddEventHandlers();
		}

		private void LoadLibrary()
		{
			if (_manager.CheckLibrary())
			{
				LibraryView.SetValue(DataGrid.ItemsSourceProperty, _manager.Library.Songs);
			}
			else
			{
				//TODO: if library empty
			}
		}

		private void StartTimer()
		{
			var updateTimer = new DispatcherTimer();
			updateTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
			updateTimer.Tick += UpdateTimer_Tick;
			updateTimer.Start();
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
			LibraryView.Columns[0].MaxWidth = 30;
		}

		private void LibraryView_Sorting(object sender, DataGridSortingEventArgs e)
		{
			var sourceCollection = LibraryView.ItemsSource as List<Song>;
			if (sourceCollection != null)
			{
				var sortDirection = e.Column.SortDirection;
				switch (sortDirection)
				{
					default:
					case ListSortDirection.Descending:
						sortDirection = ListSortDirection.Ascending;
						break;
					case ListSortDirection.Ascending:
					sortDirection = ListSortDirection.Descending;
						break;
				}

				int direction = (sortDirection == ListSortDirection.Ascending ? 1 : -1);
				Tools.Sort(sourceCollection, e.Column.SortMemberPath, direction);
			}
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

		private void UpdateTimer_Tick(object sender, EventArgs e)
		{
			//Update audiocontrols
			AudioPlayerControl.ToggleButtons();
			AudioPlayerControl.RetrieveSongInfo();
		}
	}
}
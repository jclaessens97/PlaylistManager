using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using PlaylistManager.BL;
using PlaylistManager.Domain;
using PlaylistManager.WPF.Custom;

namespace PlaylistManager.WPF
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Manager _manager;

		public Manager Manager
		{
			get => _manager;
		}

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
			
			//Datagrid events
			Style rowStyle = new Style(typeof(DataGridRow));

			rowStyle.Setters.Add(new EventSetter(DataGridRow.MouseDoubleClickEvent, new MouseButtonEventHandler(LibraryViewRow_DoubleClick)));
			
			LibraryView.RowStyle = rowStyle;
		}

		private void LibraryView_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var col in LibraryView.Columns)
			{
					col.Width = new DataGridLength(col.ActualWidth, DataGridLengthUnitType.Pixel);
			}
		}

		private void LibraryViewRow_DoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = sender as DataGridRow;
			Song song = row.Item as Song;

			if (_manager.CurrentSong != null)
			{
				_manager.Stop();
			}

			AudioPlayerControl.StartPlaying(song);
		}
	}
}
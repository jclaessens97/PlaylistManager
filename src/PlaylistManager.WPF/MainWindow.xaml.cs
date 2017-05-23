using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PlaylistManager.BL;
using PlaylistManager.Domain;

namespace PlaylistManager.WPF
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public Manager Manager { get; }

		public MainWindow()
		{
			InitializeComponent();
			Title = "PlaylistManager v0.1";
			WindowState = WindowState.Maximized;

			Manager = new Manager();
			AudioPlayerControl.Manager = Manager;

			LoadLibrary();
			AddEventHandlers();
		}

		private void LoadLibrary()
		{
			if (Manager.CheckLibrary())
			{
				foreach (var song in Manager.GetLibrarySongs())
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

			rowStyle.Setters.Add(new EventSetter(MouseDoubleClickEvent, new MouseButtonEventHandler(LibraryViewRow_DoubleClick)));
			
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
			if (row != null)
			{
				Song song = row.Item as Song;

				if (Manager.CurrentSong != null)
				{
					Manager.Stop();
				}

				AudioPlayerControl.StartPlaying(song);
			}
		}
	}
}
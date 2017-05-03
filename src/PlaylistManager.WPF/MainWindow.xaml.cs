using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PlaylistManager.BL;

namespace PlaylistManager.WPF
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Manager _manager;

		public MainWindow()
		{
			InitializeComponent();
			Title = "PlaylistManager v0.1";
			WindowState = WindowState.Maximized;

			_manager = new Manager();
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
		}

		private void LibraryView_Loaded(object sender, RoutedEventArgs e)
		{
			foreach (var col in LibraryView.Columns)
			{
					col.Width = new DataGridLength(col.ActualWidth, DataGridLengthUnitType.Pixel);
			}
		}
	}
}
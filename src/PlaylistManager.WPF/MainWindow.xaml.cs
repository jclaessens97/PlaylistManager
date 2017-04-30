using System.Windows;

namespace PlaylistManager.WPF
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			Title = "PlaylistManager v0.1";
			WindowState = WindowState.Maximized;
		}
	}
}
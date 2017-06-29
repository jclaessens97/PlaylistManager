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
		private readonly SettingsPresenter settingsPresenter;

		public MainWindow()
		{
			InitializeComponent();

			//Set the presenter of the settings control
			settingsPresenter = SettingsPresenter.Instance;
			SettingsControl.DataContext = settingsPresenter;

			//Set the presenter of the Audioplayer control
			audioplayerPresenter = new AudioplayerPresenter();
			AudioPlayerControl.DataContext = audioplayerPresenter;
			AudioPlayerControl.RegisterEvents();

			//Set the presenter of the Library control
			libraryPresenter = new LibraryPresenter();
			LibraryControl.DataContext = libraryPresenter;
			LibraryControl.LoadLibrary();
			
			//Set a reference in libraryPresenter to AudioplayerPresenter so you can access its methods (e.g. to start a song)
			libraryPresenter.AudioplayerPresenter = audioplayerPresenter;
		}

		private void TabControl_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
		{
			TabControl tabControl = _sender as TabControl;

			switch (tabControl.SelectedIndex)
			{
				//Library tab
				case 0:
					libraryBottomBar.Visibility = Visibility.Visible;
					//settingsBottomBar.Visibility = Visibility.Collapsed;
					break;
				//Playlists tab
				case 1:
					libraryBottomBar.Visibility = Visibility.Collapsed;
					//settingsBottomBar.Visibility = Visibility.Collapsed;
					break;
				//Settings tab
				case 2:
					libraryBottomBar.Visibility = Visibility.Collapsed;
					//settingsBottomBar.Visibility = Visibility.Visible;
					break;
			}
		}
	}
}

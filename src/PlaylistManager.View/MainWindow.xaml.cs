using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using PlaylistManager.ViewModel.Presenters;

namespace PlaylistManager.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Attributes

		private AudioplayerPresenter audioplayerPresenter;
		private LibraryPresenter libraryPresenter;
		private SettingsPresenter settingsPresenter;

		#endregion

		public MainWindow()
		{
			InitializeComponent();

			InitSettings();
			InitAudioPlayer();
			InitLibrary();
		}

		#region Init

		private void InitSettings()
		{
			settingsPresenter = SettingsPresenter.Instance;
			SettingsControl.DataContext = settingsPresenter;
		}

		private void InitAudioPlayer()
		{
			audioplayerPresenter = new AudioplayerPresenter();
			AudioPlayerControl.DataContext = audioplayerPresenter;
			AudioPlayerControl.RegisterEvents();
			AudioPlayerControl.LoadImplicits(SettingsPresenter.Instance);
		}

		private void InitLibrary()
		{
			libraryPresenter = new LibraryPresenter();
			LibraryControl.DataContext = libraryPresenter;
			LibraryControl.LoadLibrary();
			libraryPresenter.AudioplayerPresenter = audioplayerPresenter;
		}

		#endregion

		#region Events

		private void TabControl_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
		{
			TabControl tabControl = _sender as TabControl;

			switch (tabControl.SelectedIndex)
			{
				//Library tab
				case 0:
					libraryBottomBar.Visibility = Visibility.Visible;

					if (settingsPresenter.SettingsChanged)
					{
						libraryPresenter.ReloadSongs();
						LibraryControl.LoadLibrary();
						settingsPresenter.SettingsChanged = false;
					}

					break;
				//Playlists tab
				case 1:
					libraryBottomBar.Visibility = Visibility.Collapsed;
					break;
				//Settings tab
				case 2:
					libraryBottomBar.Visibility = Visibility.Collapsed;
					break;
			}
		}

		private void MainWindow_OnClosing(object _sender, CancelEventArgs _e)
		{
			settingsPresenter.Volume = audioplayerPresenter.Volume;
			settingsPresenter.ShuffleEnabled = audioplayerPresenter.ShuffleEnabled;
			settingsPresenter.RepeatMode = audioplayerPresenter.RepeatMode;

			settingsPresenter.SaveImplicitSettings();
		}

		#endregion
	}
}

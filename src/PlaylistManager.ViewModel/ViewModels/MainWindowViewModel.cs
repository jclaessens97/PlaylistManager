using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PlaylistManager.Model;
using PlaylistManager.Model.Other;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.Other;

namespace PlaylistManager.ViewModel.ViewModels
{
	public sealed class MainWindowViewModel : ObservableObject
	{
		#region Attributes

	    private int selectedTabIndex;

		#endregion

		#region Properties

		public IMainWindow MainWindow { get; set; }

	    public int SelectedTabIndex
		{
			get => selectedTabIndex;
			set
			{
				selectedTabIndex = value;
				RaisePropertyChangedEvent(nameof(SelectedTabIndex));
				SelectionChanged();
			}
		}

		#endregion

		#region Commands

		public ICommand CloseCommand
		{
			get
			{
				var closeCommand = new RelayCommand((arg) => OnRequestClose());
				return closeCommand;
			}
		}

		#endregion

		#region Events

		private void OnRequestClose()
		{
		    var settings = SettingsControlViewModel.Instance;
		    var player = AudioPlayerControlViewModel.Instance;

		    settings.Volume = player.Volume;
		    settings.ShuffleEnabled = player.ShuffleEnabled;
		    settings.RepeatMode = player.RepeatMode;

            settings.SaveImplicitSettings();
            Debug.WriteLine("Closing!");
		}

	    private void SelectionChanged()
	    {
	        MainWindow.SetBottomBar(SelectedTabIndex);
	    }

        #endregion

        public MainWindowViewModel()
		{
			selectedTabIndex = 0;
		}
	}
}

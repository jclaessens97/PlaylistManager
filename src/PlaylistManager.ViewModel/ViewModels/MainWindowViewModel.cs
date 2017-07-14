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
		    var player = AudioPlayerControlViewModel.audioPlayer;
            SettingsControlViewModel.SaveImplicitSettings(player.Volume * 100, player.ShuffleEnabled, (byte)player.RepeatMode);

            Debug.WriteLine("Closing!");
		}

	    private void SelectionChanged()
	    {
	        MainWindow.SetBottomBar(SelectedTabIndex);

            //Settingstab
	        if (SelectedTabIndex == 2)
	        {
                //Start validation timer
	            Messenger.Instance.Send(true, MessageContext.ToggleValidationTimer);
            }
	        else
	        {
                //Stop validation timer
	            Messenger.Instance.Send(false, MessageContext.ToggleValidationTimer);
	        }
	    }

        #endregion

        public MainWindowViewModel()
		{
			selectedTabIndex = 0;
		}
	}
}

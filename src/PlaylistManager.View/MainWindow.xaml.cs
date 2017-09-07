using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.ViewModels;

namespace PlaylistManager.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IMainWindow
	{
        #region Attributes

	    private readonly string applicationName = "PlaylistManager";
	    private Version applicationVersion()
	    {
	        try
	        {
	            return ApplicationDeployment.CurrentDeployment.CurrentVersion;
	        }
	        catch
	        {
	            return Assembly.GetExecutingAssembly().GetName().Version;
	        }
	    }

        #endregion

        #region Properties

	    public MainWindowViewModel ViewModel { get; }

        #endregion

        public MainWindow()
		{
			InitializeComponent();

            ViewModel = new MainWindowViewModel();
		    ViewModel.MainWindow = this;
		    DataContext = ViewModel;

		    Title = $"{applicationName} - v{applicationVersion()}";
		}

        #region GUI

	    /// <summary>
	    /// Changes bottombar according to selected tab index
	    /// </summary>
	    /// <param name="_tabIndex">Selected Tab Index</param>
	    public void SetBottomBar(int _tabIndex)
	    {
	        switch (_tabIndex)
	        {
	            //library tab selected
	            case 0:
	                LibraryBottomBar.Visibility = Visibility.Visible;
                    break;

	            //playlists tab selected
	            case 1:
	                LibraryBottomBar.Visibility = Visibility.Collapsed;
                    break;

	            //settings tab selected
	            case 2:
	                LibraryBottomBar.Visibility = Visibility.Collapsed;
                    break;
	        }
	    }

        #endregion
    }
}

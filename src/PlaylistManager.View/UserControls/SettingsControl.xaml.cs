using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PlaylistManager.View.Other;
using PlaylistManager.ViewModel.Presenters;
using TextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using static PlaylistManager.View.Other.MessageBoxTools;

namespace PlaylistManager.View.UserControls
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl
	{
		#region Attributes

		private readonly SettingsPresenter settingsPresenter;
	    private Timer validationTimer;
	    private Dictionary<string, object> settingsBeforeChange;

		#endregion

		public SettingsControl()
        {
            InitializeComponent();
			settingsPresenter = SettingsPresenter.Instance;
		}

		#region Validation

	    private void LoadSettingsDictionary()
	    {
			settingsBeforeChange = new Dictionary<string, object>
			{
				{"Folder", settingsPresenter.Folder},
				{"Subdirs", settingsPresenter.IncludeSubdirs},
				{"TimeBetweenSongs", settingsPresenter.TimeBetweenSongs}
			};
		}

	    internal void StartValidationTimer()
	    {
			//Get settings before anything changed & store them in a dictionary
			LoadSettingsDictionary();

			//Start timer
		    if (validationTimer == null)
	    	{
	    		validationTimer = new Timer();
	    		validationTimer.Interval = 500;
	    		validationTimer.Enabled = true;
	    		validationTimer.Tick += validationTimer_Tick;
	    	}
	    	else
	    	{
	    		validationTimer.Enabled = true;
	    	}
	    }
	    
	    internal void StopValidationTimer()
	    {
		   validationTimer?.Dispose();
	    }
	    
	    private void validationTimer_Tick(object _sender, EventArgs _e)
	    {
		    settingsPresenter.SettingsChanged = CheckSettingsChanged();
		}

		private bool CheckSettingsChanged()
		{
			if (tbFolder.Text != settingsBeforeChange["Folder"].ToString())
			{
				return true;
			}

			if (cbSubdirs.IsChecked != (bool)settingsBeforeChange["Subdirs"])
			{
				return true;
			}

			if (sliderTimeBetweenSongs.Value < (double)settingsBeforeChange["TimeBetweenSongs"]
				|| sliderTimeBetweenSongs.Value > (double)settingsBeforeChange["TimeBetweenSongs"])
			{
				return true;
			}

		    return false;
	    }

		//Only needed if textboxes aren't readonly (maybe for in the future so I leave this here)
		//
		//		private void ValidateFolderTextBox()
		//	    {
		//		    string path = tbFolder.Text;
		//		    IsValid = false;
		//
		//		    try
		//		    {
		//			    Path.GetFullPath(path); //throws exception when invalid pathname
		//
		//			    if (!Directory.Exists(path))
		//				    throw new DirectoryNotFoundException();
		//
		//			    IsValid = true;
		//		    }
		//		    catch (ArgumentException)
		//		    {
		//				//path is a zero - length string, contains only white space, 
		//				//or contains one or more of the invalid characters defined in GetInvalidPathChars.
		//				//OR
		//				//The system could not retrieve the absolute path.
		//				PauseValidationTimer();
		//				ShowErrorMsgBox("Path is not valid.\nPlease fill in a valid path.", "Invalid Path!");
		//		    }
		//		    catch (SecurityException)
		//		    {
		//				//No required permissions
		//			    PauseValidationTimer();
		//				ShowErrorMsgBox(
		//				    "You don't have the required permissions.\nTry to run as administrator or ask the system administrator!",
		//				    "No permission!");
		//			}
		//			catch (NotSupportedException)
		//		    {
		//				//Wrong character in path
		//			    PauseValidationTimer();
		//				ShowErrorMsgBox("You can't use : in a path, please fill in a valid path.", "Invalid character!");
		//			}
		//			catch (PathTooLongException)
		//		    {
		//				//Path too long
		//			    PauseValidationTimer();
		//				ShowErrorMsgBox("The specified path exceeds the maximum characters!", "Path too long!");
		//			}
		//			catch (DirectoryNotFoundException)
		//		    {
		//				//Directory doesn't exist
		//			    PauseValidationTimer();
		//				ShowErrorMsgBox("Directory doesn't exist!\nTry again with an existing directory!", "Directory doesn't exist!");
		//			    tbFolder.Focus();
		//				StartValidationTimer();
		//			}
		//			catch (Exception ex)
		//		    {
		//				//Other exceptions
		//			    PauseValidationTimer();
		//				ShowErrorMsgBox("Something went wrong!");
		//				Debug.WriteLine(ex);
		//		    }
		//	    }

		#endregion

	    #region Button Events

		private void BtnBrowseFolder_OnClick(object _sender, RoutedEventArgs _e)
		{
			var presenter = SettingsPresenter.Instance;

			var dialog = new FolderBrowserDialog()
			{
				SelectedPath = presenter.Folder,
				ShowNewFolderButton = false
			};

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				presenter.Folder = dialog.SelectedPath;
				Debug.WriteLine(presenter.Folder);
			}
		}

	    private void BtnSave_OnClick(object _sender, RoutedEventArgs _e)
	    {
		    LoadSettingsDictionary();
	    }

		#endregion
	}
}

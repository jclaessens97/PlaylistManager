﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using PlaylistManager.ViewModel.Presenters;
using UserControl = System.Windows.Controls.UserControl;

namespace PlaylistManager.View.UserControls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
		public SettingsControl()
        {
            InitializeComponent();
		}

	    private void BtnBrowseFolder_OnClick(object _sender, RoutedEventArgs _e)
	    {
			var presenter = SettingsPresenter.Instance;

			var dialog = new FolderBrowserDialog();
		    dialog.SelectedPath = presenter.Folder;
		    dialog.ShowNewFolderButton = false;

		    var result = dialog.ShowDialog();

		    if (result == DialogResult.OK)
		    {
			    presenter.Folder = dialog.SelectedPath;
				Debug.WriteLine(presenter.Folder);
		    }
	    }

	    private void TbFolder_OnLostFocus(object _sender, RoutedEventArgs _e)
	    {
			//TODO: validate path
		}
    }
}

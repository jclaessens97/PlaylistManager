using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using PlaylistManager.View.Other;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.ViewModels;
using TextBox = System.Windows.Controls.TextBox;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;
using static PlaylistManager.View.Other.MessageBoxTools;
using CheckBox = System.Windows.Controls.CheckBox;

namespace PlaylistManager.View.UserControls
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl, ISettingsControl
	{
        #region Properties

	    public TextBox FolderTextBox => tbFolder;
	    public CheckBox SubDirsCheckbox => cbSubdirs;
	    public ToggleButton ThemeToggleButton => toggleTheme;
	    public Slider TimeBetweenSongsSlider => sliderTimeBetweenSongs;

        #endregion

	    public SettingsControl()
	    {
	        InitializeComponent();

	        DataContext = SettingsControlViewModel.Instance;
	        (DataContext as SettingsControlViewModel).SettingsControl = this;
	    }
	}
}

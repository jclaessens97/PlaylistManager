using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.ViewModels;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;
using CheckBox = System.Windows.Controls.CheckBox;

namespace PlaylistManager.View.UserControls
{
	/// <summary>
	/// Interaction logic for SettingsControl.xaml
	/// </summary>
	public partial class SettingsControl : UserControl, ISettingsControl
	{
        #region Properties

	    public SettingsControlViewModel ViewModel { get; }
	    public TextBox FolderTextBox => tbFolder;
	    public CheckBox SubDirsCheckbox => cbSubdirs;
	    public ToggleButton ThemeToggleButton => toggleTheme;
	    public Slider TimeBetweenSongsSlider => sliderTimeBetweenSongs;

        #endregion

	    public SettingsControl()
	    {
	        InitializeComponent();

            ViewModel = new SettingsControlViewModel();
	        ViewModel.SettingsControl = this;
	        DataContext = ViewModel;
	    }
	}
}

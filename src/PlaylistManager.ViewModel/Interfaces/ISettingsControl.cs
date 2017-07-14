using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using PlaylistManager.ViewModel.ViewModels;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface ISettingsControl
    {
        SettingsControlViewModel ViewModel { get; }
        TextBox FolderTextBox { get; }
        CheckBox SubDirsCheckbox { get; }
        ToggleButton ThemeToggleButton { get; }
        Slider TimeBetweenSongsSlider { get; }
    }
}
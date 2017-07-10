using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface ISettingsControl
    {
        TextBox FolderTextBox { get; }
        CheckBox SubDirsCheckbox { get; }
        ToggleButton ThemeToggleButton { get; }
        Slider TimeBetweenSongsSlider { get; }
    }
}
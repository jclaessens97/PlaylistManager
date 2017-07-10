using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface IAudioPlayerControl
    {
        PackIcon PlayPauseIcon { get; }
        PackIcon RepeatIcon { get; }
        PackIcon ShuffleIcon { get; }
        PackIcon VolumeIcon { get; }
        void ToggleEnable(bool _state);
    }
}

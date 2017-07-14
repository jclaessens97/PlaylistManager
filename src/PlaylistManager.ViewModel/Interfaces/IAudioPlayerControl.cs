using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using PlaylistManager.ViewModel.ViewModels;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface IAudioPlayerControl
    {
        AudioPlayerControlViewModel ViewModel { get; }
        PackIcon PlayPauseIcon { get; }
        PackIcon RepeatIcon { get; }
        PackIcon ShuffleIcon { get; }
        PackIcon VolumeIcon { get; }
        void ToggleEnable(bool _state);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.ViewModel.Other
{
    internal enum MessageContext : byte
    {
        //LibraryVM -> AudioPlayerVM
        PlayOneSongDoubleClick = 0,

        //MainVM -> SettingsVM
        ToggleValidationTimer
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaylistManager.ViewModel.ViewModels;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface IMainWindow
    {
        MainWindowViewModel ViewModel { get; }
	    void SetBottomBar(int _tabIndex);
    }
}

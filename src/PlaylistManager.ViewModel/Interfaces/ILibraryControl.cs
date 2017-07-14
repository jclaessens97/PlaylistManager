using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using PlaylistManager.ViewModel.ViewModels;

namespace PlaylistManager.ViewModel.Interfaces
{
    public interface ILibraryControl
    {
        LibraryControlViewModel ViewModel { get; }
        DataGrid LibraryDataGrid { get; }
    }
}

using System.Windows.Controls;
using PlaylistManager.ViewModel.Interfaces;
using PlaylistManager.ViewModel.ViewModels;


namespace PlaylistManager.View.UserControls
{
    /// <summary>
    /// Interaction logic for LibraryControl.xaml
    /// </summary>
    public partial class LibraryControl : UserControl, ILibraryControl
    {
        #region Properties

        public DataGrid LibraryDataGrid => libraryDataGrid;

        #endregion

        public LibraryControl()
        {
            InitializeComponent();

            DataContext = LibraryControlViewModel.Instance;
            (DataContext as LibraryControlViewModel).LibraryControl = this;
            (DataContext as LibraryControlViewModel).FillLibraryGrid();
        }
	}
}

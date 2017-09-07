using System.ComponentModel;
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

        public LibraryControlViewModel ViewModel { get; }
        public DataGrid LibraryDataGrid => libraryDataGrid;

        #endregion

        public LibraryControl()
        {
            InitializeComponent();

            ViewModel = new LibraryControlViewModel();
            ViewModel.LibraryControl = this;
            ViewModel.FillLibraryGrid();
            this.TitleColumn.SortDirection = ListSortDirection.Ascending;

            DataContext = ViewModel;
        }
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using PlaylistManager.ViewModel;

namespace PlaylistManager.View
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

//			//Create ViewModel & Attach it to the window
//			MainWindow window = new MainWindow();
//			var viewModel = new MainWindowViewModel();
//
//			//Create handler that allows viewmodel to close the window
//			EventHandler handler = null;
//			handler = delegate
//			{
//				//TODO: launch messagebox "are you sure?"
//				//Until there is a messagebox asking for confirmation, just always close it 
//				bool resultClose = true;
//
//				if (resultClose)
//				{
//					//viewModel.RequestClose -= handler;
//					window.Close();
//				}
//			};
//
//			viewModel.RequestClose += handler;
//			window.Show();
		}
	}
}

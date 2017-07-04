using System;
using System.Windows.Input;

namespace PlaylistManager.ViewModel.Other
{
	public class DelegateCommand : ICommand
	{
		#pragma warning disable 67
				public event EventHandler CanExecuteChanged;
		#pragma warning restore 67

		private readonly Action action;

		public DelegateCommand(Action _action)
		{
			action = _action;
		}

		public bool CanExecute(object _parameter)
		{
			return true;
		}

		public void Execute(object _parameter)
		{
			this.action();
		}
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylistManager.ViewModel
{
	public class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChangedEvent(string _propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(_propertyName));
		}
	}
}
